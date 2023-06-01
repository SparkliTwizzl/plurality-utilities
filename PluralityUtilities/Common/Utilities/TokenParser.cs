using PluralityUtilities.Common.Containers;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class TokenParser
	{
		public int IndentLevel { get; set; } = 0;

		public TokenParser() { }


		public QualifiedToken ParseToken( string token, string expectedValue )
		{
			Log.WriteLineTimestamped( $"started parsing Token \"{ token }\", expecting value \"{ expectedValue }\"");
			var trimmedToken = token.Trim();
			QualifiedToken qualifiedToken = new QualifiedToken();
			qualifiedToken.Token = trimmedToken;

			if ( string.Compare( trimmedToken, "{" ) == 0 )
			{
				++IndentLevel;
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.OpenBracket;
			}
			else if ( string.Compare( token, "}" ) == 0 )
			{
				--IndentLevel;
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.CloseBracket;
			}
			else if ( string.Compare( token, "" ) == 0 )
			{
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.BlankLine;
			}
			else if ( string.Compare( token, expectedValue ) == 0 )
			{
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.Recognized;
			}
			else
			{
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.Unknown;
			}

			return qualifiedToken;
		}
	}
}
