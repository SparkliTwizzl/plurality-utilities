using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public static class TemplateHandler
	{
		private static class FindAndReplaceParseHandler
		{
			public static Dictionary<string, string> ParseFindKeys( StringToken token )
			{
				var result = new Dictionary<string, string>();
				var items = ExtractItemsFromBody( token );
				foreach ( var item in items )
				{
					result.Add( item, string.Empty );
				}
				return result;
			}

			public static Dictionary<string, string> ParseReplaceValues( StringToken token, Dictionary<string, string> result )
			{
				if ( result.Count < 1 )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{Tokens.Template.Key}\" region has a \"{token.Key}\" token, but is missing a corresponding \"{Tokens.Find}\" token." ), token.LineNumber );
				}

				var items = ExtractItemsFromBody( token );
				if ( items.Length != result.Count )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{token.Key}\" token's body has a different number of items than its corresponding \"{Tokens.Find.Key}\" token (has: {items.Length} / should have: {result.Count})." ), token.LineNumber );
				}

				for ( var i = 0 ; i < items.Length ; ++i )
				{
					var key = result.ElementAt( i ).Key;
					result[ key ] = items[ i ];
				}

				return result;
			}


			private static string[] ExtractItemsFromBody( StringToken token )
			{
				if ( token.Value == string.Empty )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.Key}\" token values cannot be blank." ), token.LineNumber );
				}

				if ( !token.Value.StartsWith( Common.Syntax.ControlSequences.RegionOpen ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.Key}\" token bodies must start with a '{Common.Syntax.ControlSequences.RegionOpen}' character." ), token.LineNumber );
				}

				if ( !token.Value.EndsWith( Common.Syntax.ControlSequences.RegionClose ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.Key}\" token bodies must end with a '{Common.Syntax.ControlSequences.RegionClose}' character." ), token.LineNumber );
				}

				var items = token.Value[ 1..( token.Value.Length - 1 ) ]
					.Split( ',' );

				for ( var i = 0 ; i < items.Length ; ++i )
				{
					items[ i ] = items[ i ].Trim();
				}

				if ( items.Contains( string.Empty ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.Key}\" token bodies cannot contain blank items." ), token.LineNumber );
				}

				return items;
			}
		}


		private static class TemplateParseHandler
		{
			public static ScriptMacroTemplate ParseTemplateString( StringToken token, ScriptMacroTemplate result )
			{
				ValidateTemplateStructure( token );
				var sanitizedHotstring = SanitizeHotstring( token.Value );

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

				var components = template.ToString().Split( ControlSequences.TemplateFindReplaceDivider );
				result.TemplateFindString = components[ 0 ].Trim();
				result.TemplateReplaceString = components[ 1 ].Trim();
				return result;
			}


			private static void ValidateTemplateStructure( StringToken token )
			{
				var components = token.Value.Split( ControlSequences.TemplateFindReplaceDivider );
				var doesFindStringExist = components[ 0 ]?.Length > 0;
				var doesReplaceStringExist = components[ 1 ]?.Length > 0;
				var isTemplateInValidFormat = doesFindStringExist && doesReplaceStringExist;
				if ( !isTemplateInValidFormat )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{Tokens.Template.Key}\" token's value is not a valid template string." ), token.LineNumber );
				}
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

			private static string SanitizeHotstring( string rawHotstring )
				=> rawHotstring
					.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.Escape}", Common.Syntax.ControlSequences.EscapeStandin )
					.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}", Common.Syntax.ControlSequences.FindTagOpenStandin )
					.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}", Common.Syntax.ControlSequences.FindTagCloseStandin );

			private static void ValidateFindTagValue( string findTag, int lineNumber )
			{
				if ( !TemplateFindTags.LookUpTable.Contains( findTag ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A template string contains an unrecognized \"find\" tag value ( \"{findTag}\" )." ), lineNumber );
				}
			}
		}


		private static class TextCaseParseHandler
		{
			public static string ParseTextCase( StringToken token )
			{
				if ( token.Value == string.Empty )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.Key}\" token values cannot be blank." ), token.LineNumber );
				}

				if ( !TemplateTextCases.LookUpTable.Contains( token.Value ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{token.Key}\" token's value was not recognized." ), token.LineNumber );
				}

				return token.Value;
			}
		}


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
		/// Thrown when a token's body has no region open character.
		/// Thrown when a token's body has no region close character.
		/// Thrown when a token's body contains blank items.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> FindTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.FindAndReplace = FindAndReplaceParseHandler.ParseFindKeys( token );
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
		/// Thrown when a token is not preceeded by a <see cref="Tokens.Find"/> token.
		/// Thrown when a token's value has no body.
		/// Thrown when a token's body has no region open character.
		/// Thrown when a token's body has no region close character.
		/// Thrown when a token's body contains blank items.
		/// Thrown when a token's body contains less items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// Thrown when a token's body contains more items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> ReplaceTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.FindAndReplace = FindAndReplaceParseHandler.ParseReplaceValues( token, result.FindAndReplace );
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
			result = TemplateParseHandler.ParseTemplateString( token, result );
			return new ProcessedRegionData<ScriptMacroTemplate>( result );
		}

		/// <summary>
		/// Converts <see cref="Tokens.TextCase"/> token values into text case converstion modes.
		/// </summary>
		/// <param name="regionData">Untrimmed input data.</param>
		/// <param name="tokenStartIndex">Index within input data of token to process.</param>
		/// <param name="result">Existing result to modify and return.</param>
		/// <returns>Modified <paramref name="result"/> with "replace" values.</returns>
		///
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value has no body.
		/// Thrown when a token's value is not recognized.
		/// </exception>
		public static ProcessedRegionData<ScriptMacroTemplate> TextCaseTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.TextCase = TextCaseParseHandler.ParseTextCase( token );
			return new ProcessedRegionData<ScriptMacroTemplate>( result );
		}
	}
}
