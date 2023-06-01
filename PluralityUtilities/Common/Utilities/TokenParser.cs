using PluralityUtilities.Common.Containers;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class TokenParser
	{
		public int IndentLevel { get; set; } = 0;

		public TokenParser() { }


		public QualifiedToken ParseToken( string token, string[] expectedValues )
		{
			Log.WriteLineTimestamped( $"started parsing token \"{ token }\", expecting a value from:");
			foreach ( var tokenValue in expectedValues )
			{
				Log.WriteLineTimestamped( $"    { tokenValue }" );
			}

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
			else
			{
				for ( var i = 0; i < expectedValues.Length; ++i )
				{
					if ( string.Compare( token, expectedValues[ i ] ) == 0 )
					{
						qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.Recognized;
						break;
					}
				}
			}

			return qualifiedToken;
		}
	}
}
