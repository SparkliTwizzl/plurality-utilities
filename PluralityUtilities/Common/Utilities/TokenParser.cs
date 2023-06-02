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
				Log.WriteLineTimestamped( $"	{ tokenValue }" );
			}

			var qualifiedToken = new QualifiedToken( token.Trim() );
			if ( string.Compare( qualifiedToken.Token, "{" ) == 0 )
			{
				++IndentLevel;
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.OpenBracket;
			}
			else if ( string.Compare( qualifiedToken.Token, "}" ) == 0 )
			{
				--IndentLevel;
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.CloseBracket;
			}
			else if ( string.Compare( qualifiedToken.Token, "" ) == 0 )
			{
				qualifiedToken.Qualifier = Common.Enums.TokenQualifiers.BlankLine;
			}
			else
			{
				for ( var i = 0; i < expectedValues.Length; ++i )
				{
					if ( string.Compare( qualifiedToken.Token, expectedValues[ i ] ) == 0 )
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
