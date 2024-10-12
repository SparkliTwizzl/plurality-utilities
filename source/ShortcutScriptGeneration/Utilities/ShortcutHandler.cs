using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Provides methods to handle parsing text shortcut tokens.
	/// </summary>
	public static class ShortcutHandler
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
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{TokenPrototypes.ShortcutTemplate.Key}\" region has a \"{token.TokenKey}\" token, but is missing a corresponding \"{TokenPrototypes.Find}\" token." ), token.LineNumber );
				}

				var items = ExtractItemsFromBody( token );
				if ( items.Length != result.Count )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{token.TokenKey}\" token's body has a different number of items than it's corresponding \"{TokenPrototypes.Find.Key}\" token (has: {items.Length} / should have: {result.Count})." ), token.LineNumber );
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
				if ( token.TokenValue == string.Empty )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.TokenKey}\" token values cannot be blank." ), token.LineNumber );
				}

				if ( !token.TokenValue.StartsWith( Common.Syntax.ControlSequences.TokenBodyOpen ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.TokenKey}\" token bodies must start with a '{Common.Syntax.ControlSequences.TokenBodyOpen}' character." ), token.LineNumber );
				}

				if ( !token.TokenValue.EndsWith( Common.Syntax.ControlSequences.TokenBodyClose ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.TokenKey}\" token bodies must end with a '{Common.Syntax.ControlSequences.TokenBodyClose}' character." ), token.LineNumber );
				}

				var items = token.TokenValue[ 1..( token.TokenValue.Length - 1 ) ]
					.Split( ',' );

				for ( var i = 0 ; i < items.Length ; ++i )
				{
					items[ i ] = items[ i ].Trim();
				}

				if ( items.Contains( string.Empty ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.TokenKey}\" token bodies cannot contain blank items." ), token.LineNumber );
				}

				return items;
			}
		}


		private static class ShortcutParseHandler
		{
			public static TextShortcut ParseTemplateString( StringToken token, TextShortcut result )
			{
				ValidateShortcutStructure( token );
				var sanitizedHotstring = SanitizeHotstring( token.TokenValue );

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

				var components = template.ToString().Split( ControlSequences.TemplateFindStringDelimiter );
				result.TemplateFindString = components[ 0 ].Trim();
				result.TemplateReplaceString = components[ 1 ].Trim();
				return result;
			}

			public static void ValidateShortcutStructure( StringToken token )
			{
				var hotstring = SanitizeHotstring( token.TokenValue );
				var components = hotstring.Split( ControlSequences.TemplateFindStringDelimiter );
				var doesFindStringExist = ( components.Length > 0 ) && ( components[ 0 ]?.Length > 0 );
				var doesReplaceStringExist = ( components.Length > 1 ) && ( components[ 1 ]?.Length > 0 );
				var isTemplateInValidFormat = doesFindStringExist && doesReplaceStringExist;
				if ( !isTemplateInValidFormat )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{token.TokenKey}\" token's value is not a valid shortcut string." ), token.LineNumber );
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

			private static string SanitizeHotstring( string rawHotstring ) => rawHotstring.EscapedCharsToCodepoints();

			private static void ValidateFindTagValue( string findTag, int lineNumber )
			{
				if ( !TemplateFindTags.LookupTable.Contains( findTag ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A template string contains an unrecognized \"find\" tag value ( \"{findTag}\" )." ), lineNumber );
				}
			}
		}


		private static class TextCaseParseHandler
		{
			public static string ParseTextCase( StringToken token )
			{
				if ( token.TokenValue == string.Empty )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"\"{token.TokenKey}\" token values cannot be blank." ), token.LineNumber );
				}

				if ( !TemplateTextCases.LookupTable.Contains( token.TokenValue ) )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A(n) \"{token.TokenKey}\" token's value was not recognized." ), token.LineNumber );
				}

				return token.TokenValue;
			}
		}


		/// <summary>
		/// Converts <see cref="TokenPrototypes.Find"/> token values into find-and-replace dictionary keys.
		/// </summary>
		/// <param name="bodyData">Indexed strings representing body data.</param>
		/// <param name="tokenStartIndex">Start index of token within body data.</param>
		/// <param name="result">Result to modify.</param>
		/// <returns>Modified result with "find" keys.</returns>
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value has no body.
		/// Thrown when a token's body has no region open character.
		/// Thrown when a token's body has no region close character.
		/// Thrown when a token's body contains blank items.
		/// </exception>
		public static ProcessedTokenData<TextShortcut> FindTokenHandler( IndexedString[] bodyData, int tokenStartIndex, TextShortcut result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			result.FindAndReplace = FindAndReplaceParseHandler.ParseFindKeys( token );
			return new ProcessedTokenData<TextShortcut>( result );
		}

		/// <summary>
		/// Converts <see cref="TokenPrototypes.Replace"/> token values into find-and-replace dictionary values.
		/// </summary>
		/// <param name="bodyData">Indexed strings representing body data.</param>
		/// <param name="tokenStartIndex">IStart index of token within body data.</param>
		/// <param name="result">Result to modify.</param>
		/// <returns>Modified result with "replace" values.</returns>
		/// <exception cref="TokenValueException">
		/// Thrown when a token is not preceeded by a <see cref="TokenPrototypes.Find"/> token.
		/// Thrown when a token's value has no body.
		/// Thrown when a token's body has no region open character.
		/// Thrown when a token's body has no region close character.
		/// Thrown when a token's body contains blank items.
		/// Thrown when a token's body contains less items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// Thrown when a token's body contains more items than are in the find-and-replace dictionary of <paramref name="result"/>.
		/// </exception>
		public static ProcessedTokenData<TextShortcut> ReplaceTokenHandler( IndexedString[] bodyData, int tokenStartIndex, TextShortcut result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			result.FindAndReplace = FindAndReplaceParseHandler.ParseReplaceValues( token, result.FindAndReplace );
			return new ProcessedTokenData<TextShortcut>( result );
		}

		/// <summary>
		/// Stores <see cref="TokenPrototypes.Shortcut"/> token values in the provided container.
		/// </summary>
		/// <param name="bodyData">Indexed strings representing body data.</param>
		/// <param name="tokenStartIndex">IStart index of token within body data.</param>
		/// <param name="result">Result to modify.</param>
		/// <returns>Modified result with converted template string.</returns>
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value is not a valid template string.
		/// </exception>
		public static ProcessedTokenData<ShortcutScriptInput> ShortcutTokenHandler( IndexedString[] bodyData, int tokenStartIndex, ShortcutScriptInput result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			ShortcutParseHandler.ValidateShortcutStructure( token );
			var shortcuts = result.Shortcuts.ToList();
			var newShortcut = token.TokenValue.EscapedCharsToCodepoints();
			shortcuts.Add( newShortcut );
			result.Shortcuts = shortcuts.ToArray();
			return new ProcessedTokenData<ShortcutScriptInput>()
			{
				Value = result,
			};
		}

		/// <summary>
		/// Converts <see cref="TokenPrototypes.ShortcutTemplate"/> token values into Petrichor template strings and stores them in the provided container.
		/// </summary>
		/// <param name="bodyData">Indexed strings representing body data.</param>
		/// <param name="tokenStartIndex">IStart index of token within body data.</param>
		/// <param name="result">Result to modify.</param>
		/// <returns>Modified result with converted template string.</returns>
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value is not a valid template string.
		/// Thrown when a token's value contains a dangling escape character.
		/// Thrown when a token's value contains a malformed "find" tag:
		/// - Mismatched tag open character.
		/// - Mismatched tag close character.
		/// - Unrecognized tag value.
		/// </exception>
		public static ProcessedTokenData<TextShortcut> ShortcutTemplateTokenHandler( IndexedString[] bodyData, int tokenStartIndex, TextShortcut result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			result = ShortcutParseHandler.ParseTemplateString( token, result );
			return new ProcessedTokenData<TextShortcut>( result );
		}

		/// <summary>
		/// Converts <see cref="TokenPrototypes.TextCase"/> token values into text case converstion modes.
		/// </summary>
		/// <param name="bodyData">Indexed strings representing body data.</param>
		/// <param name="tokenStartIndex">IStart index of token within body data.</param>
		/// <param name="result">Result to modify.</param>
		/// <returns>Modified result with "replace" values.</returns>
		/// <exception cref="TokenValueException">
		/// Thrown when a token's value has no body.
		/// Thrown when a token's value is not recognized.
		/// </exception>
		public static ProcessedTokenData<TextShortcut> TextCaseTokenHandler( IndexedString[] bodyData, int tokenStartIndex, TextShortcut result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			result.TextCase = TextCaseParseHandler.ParseTextCase( token );
			return new ProcessedTokenData<TextShortcut>( result );
		}
	}
}
