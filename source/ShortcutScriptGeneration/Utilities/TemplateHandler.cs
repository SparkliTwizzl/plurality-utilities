using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public static class TemplateHandler
	{
		/// <summary>
		/// Converts template token values into Petrichor template strings.
		/// </summary>
		/// <param name="regionData">Untrimmed input data.</param>
		/// <param name="tokenStartIndex">Index within input data of token to process.</param>
		/// <param name="result">Existing result to modify and return.</param>
		/// <returns>Modified <paramref name="result"/>.</returns>
		///
		/// <exception cref="TokenValueException">
		/// Thrown when a token value contains a dangling escape character
		/// Thrown when a token value contains a malformed "find" string:
		/// - Mismatched tag open character.
		/// - Mismatched tag close character.
		/// - Unrecognized tag value.
		/// </exception>
		public static ProcessedRegionData<List<string>> TokenHandler( IndexedString[] regionData, int tokenStartIndex, List<string> result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.Add( ParseTemplateFromLine( token.Value, token.LineNumber ) );
			return new ProcessedRegionData<List<string>>()
			{
				Value = result,
			};
		}


		private static string ConvertTemplateToAutoHotkeySyntax( string line )
		{
			var components = line.Split( "::" );
			var findString = $"::{components[ 0 ].Trim()}::";
			var replaceString = components.Length > 1 ? components[ 1 ].Trim() : "";
			return $"{findString}{replaceString}";
		}

		private static string ExtractFindString( string input, int lineNumber )
		{
			var lengthOfFindString = GetLengthOfFindString( input, lineNumber );
			return input[ ..( lengthOfFindString + 1 ) ];
		}

		private static int GetIndexOfNextFindStringCloseChar( string input, int lineNumber )
		{
			var nextCloseCharIndex = input.IndexOf( Common.Syntax.OperatorChars.TokenNameClose );
			if ( nextCloseCharIndex < 0 )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched find-string open character ( '{Common.Syntax.OperatorChars.TokenNameOpen}' )." ), lineNumber );
			}

			var isCloseCharEscaped = input[ nextCloseCharIndex - 1 ] == '\\';
			if ( isCloseCharEscaped )
			{
				var substring = input[ ( nextCloseCharIndex + 1 ).. ];
				return nextCloseCharIndex + GetIndexOfNextFindStringCloseChar( substring, lineNumber );
			}

			return nextCloseCharIndex;
		}

		private static int GetLengthOfFindString( string input, int lineNumber ) => GetIndexOfNextFindStringCloseChar( input, lineNumber );

		private static string ParseTemplateFromLine( string line, int lineNumber )
		{
			var rawHotstring = ConvertTemplateToAutoHotkeySyntax( line );
			var sanitizedHotstring = SanitizeHotstring( rawHotstring );

			var template = new StringBuilder();
			for ( var i = 0 ; i < sanitizedHotstring.Length ; ++i )
			{
				var c = sanitizedHotstring[ i ];

				if ( c == Common.Syntax.OperatorChars.TokenNameClose )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched find-string close character ('{Common.Syntax.OperatorChars.TokenNameClose}')." ), lineNumber );
					break;
				}

				else if ( c == Common.Syntax.OperatorChars.TokenNameOpen )
				{
					var substring = sanitizedHotstring[ i.. ];
					var findString = ExtractFindString( substring, lineNumber );
					ValidateFindString( findString, lineNumber );
					_ = template.Append( findString );
					i += findString.Length - 1;
				}

				else if ( c == Common.Syntax.OperatorChars.Escape )
				{
					try
					{
						_ = template.Append( sanitizedHotstring[ i..( i + 2 ) ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a dangling escape character ('{Common.Syntax.OperatorChars.Escape}') with no following character to escape.", exception ), lineNumber );
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
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.Escape}", Common.Syntax.OperatorChars.EscapeStandin )
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.TokenNameOpen}", Common.Syntax.OperatorChars.TokenNameOpenStandin )
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.TokenNameClose}", Common.Syntax.OperatorChars.TokenNameCloseStandin );

		private static void ValidateFindString( string findString, int lineNumber )
		{
			if ( !ScriptTemplateFindStrings.LookUpTable.Contains( findString ) )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained an unknown \"find\" string ( \"{findString}\" )." ), lineNumber );
			}
		}
	}
}
