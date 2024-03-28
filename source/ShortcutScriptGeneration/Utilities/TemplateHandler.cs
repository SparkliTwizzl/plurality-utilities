using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using Petrichor.ShortcutScriptGeneration.Syntax;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public static class TemplateHandler
	{
		/// <summary>
		/// Converts <see cref="Tokens.Find"/> token values into find-and-replace dictionary keys.
		/// </summary>
		/// <param name="regionData">Untrimmed input data.</param>
		/// <param name="tokenStartIndex">Index within input data of token to process.</param>
		/// <param name="result">Existing result to modify and return.</param>
		/// <returns>Modified <paramref name="result"/> with "find" keys.</returns>
		///
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value has no body.
		/// Thrown when a token's value has no region open character.
		/// Thrown when a token's value has no region close character.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> FindTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.FindAndReplace = ParseFindKeysFromStringToken( token );
			return new ProcessedRegionData<ScriptMacroTemplate>( result );
		}

		/// <summary>
		/// Converts <see cref="Tokens.Replace"/> token values into find-and-replace dictionary values.
		/// </summary>
		/// <param name="regionData">Untrimmed input data.</param>
		/// <param name="tokenStartIndex">Index within input data of token to process.</param>
		/// <param name="result">Existing result to modify and return.</param>
		/// <returns>Modified <paramref name="result"/> with "replace" values.</returns>
		///
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value has no body.
		/// Thrown when a token's value has no region open character.
		/// Thrown when a token's value has no region close character.
		/// Thrown when a token's value contains less items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// Thrown when a token's value contains more items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> ReplaceTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.FindAndReplace = ParseReplaceValuesFromStringToken( token, result );
			return new ProcessedRegionData<ScriptMacroTemplate>( result );
		}

		/// <summary>
		/// Converts <see cref="Tokens.Template"/> token values into Petrichor template strings and stores them in the provided container.
		/// </summary>
		/// <param name="regionData">Untrimmed input data.</param>
		/// <param name="tokenStartIndex">Index within input data of token to process.</param>
		/// <param name="result">Existing result to modify and return.</param>
		/// <returns>Modified <paramref name="result"/> with converted template string.</returns>
		///
		/// <exception cref="TokenValueException">
		/// Thrown when token's value is not a valid template string.
		/// Thrown when token's value contains a dangling escape character.
		/// Thrown when token's value contains a malformed "find" tag:
		/// - Mismatched tag open character.
		/// - Mismatched tag close character.
		/// - Unrecognized tag value.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> TemplateTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.TemplateString = ParseTemplateFromStringToken( token );
			return new ProcessedRegionData<ScriptMacroTemplate>( result );
		}


		private static string ConvertTemplateToAutoHotkeySyntax( string template, int lineNumber )
		{
			var components = template.Split( ControlSequences.TemplateFindReplaceDivider );
			var doesFindStringExist = components[ 0 ]?.Length > 0;
			var doesReplaceStringExist = components[ 1 ]?.Length > 0;
			var isTemplateInValidFormat = doesFindStringExist && doesReplaceStringExist;
			if ( !isTemplateInValidFormat )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{Tokens.Template.Key}\" token's value is not a valid template string." ), lineNumber );
			}

			var findString = components[ 0 ].Trim();
			var replaceString = components[ 1 ].Trim();
			return $"::{findString}::{replaceString}";
		}

		private static string ExtractFindTagFromLine( string line, int lineNumber )
		{
			var lengthOfFindString = GetLengthOfFindTag( line, lineNumber );
			return line[ ..lengthOfFindString ];
		}

		private static int GetIndexOfNextFindTagCloseChar( string line, int lineNumber )
		{
			var nextCloseCharIndex = line.IndexOf( Common.Syntax.ControlSequences.FindTagClose );
			var nextOpenCharIndex = line.IndexOf( Common.Syntax.ControlSequences.FindTagOpen );

			var hasNoCloseChar = nextCloseCharIndex < 0;
			var hasMismatchedOpenChar = nextOpenCharIndex >= 0 && nextCloseCharIndex > nextOpenCharIndex;
			if ( hasNoCloseChar || hasMismatchedOpenChar )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched \"find\" tag open character ( '{Common.Syntax.ControlSequences.FindTagOpen}' )." ), lineNumber );
			}

			var isCloseCharEscaped = line[ nextCloseCharIndex - 1 ] == Common.Syntax.ControlSequences.Escape;
			if ( isCloseCharEscaped )
			{
				var substring = line[ ( nextCloseCharIndex + 1 ).. ];
				return nextCloseCharIndex + GetIndexOfNextFindTagCloseChar( substring, lineNumber );
			}

			return nextCloseCharIndex;
		}

		private static int GetLengthOfFindTag( string findTag, int lineNumber )
		{
			var findTagOpenCharTrimmed = findTag[ 1.. ];
			var findTagValueLength = GetIndexOfNextFindTagCloseChar( findTagOpenCharTrimmed, lineNumber );
			return findTagValueLength + 2;
		}

		private static Dictionary<string, string> ParseFindKeysFromStringToken( StringToken token )
		{
			var result = new Dictionary<string, string>();
			return result;
		}

		private static Dictionary<string, string> ParseReplaceValuesFromStringToken( StringToken token, ScriptMacroTemplate result )
		{
			return result.FindAndReplace;
		}

		private static string ParseTemplateFromStringToken( StringToken token )
		{
			var rawHotstring = ConvertTemplateToAutoHotkeySyntax( token.Value, token.LineNumber );
			var sanitizedHotstring = SanitizeHotstring( rawHotstring );

			var template = new StringBuilder();
			for ( var i = 0 ; i < sanitizedHotstring.Length ; ++i )
			{
				var c = sanitizedHotstring[ i ];

				if ( c == Common.Syntax.ControlSequences.FindTagClose )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched find-string close character ('{Common.Syntax.ControlSequences.FindTagClose}')." ), token.LineNumber );
					break;
				}

				else if ( c == Common.Syntax.ControlSequences.FindTagOpen )
				{
					var substring = sanitizedHotstring[ i.. ];
					var findString = ExtractFindTagFromLine( substring, token.LineNumber );
					ValidateFindTagValue( findString, token.LineNumber );
					_ = template.Append( findString );
					i += findString.Length - 1;
				}

				else if ( c == Common.Syntax.ControlSequences.Escape )
				{
					try
					{
						_ = template.Append( sanitizedHotstring[ i..( i + 2 ) ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a dangling escape character ('{Common.Syntax.ControlSequences.Escape}') with no following character to escape.", exception ), token.LineNumber );
						break;
					}
				}

				else
				{
					_ = template.Append( c );
				}
			}
			return template.ToString();
		}

		private static string SanitizeHotstring( string rawHotstring )
			=> rawHotstring
				.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.Escape}", Common.Syntax.ControlSequences.EscapeStandin )
				.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}", Common.Syntax.ControlSequences.FindTagOpenStandin )
				.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}", Common.Syntax.ControlSequences.FindTagCloseStandin );

		private static void ValidateFindTagValue( string findTag, int lineNumber )
		{
			if ( !ScriptTemplateFindStrings.LookUpTable.Contains( findTag ) )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template string contains an unrecognized \"find\" tag value ( \"{findTag}\" )." ), lineNumber );
			}
		}
	}
}
