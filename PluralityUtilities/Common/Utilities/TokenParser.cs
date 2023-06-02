using PluralityUtilities.Common.Containers;
using PluralityUtilities.Common.Enums;
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
				qualifiedToken.Qualifier = TokenQualifiers.OpenBracket;
			}
			else if ( string.Compare( qualifiedToken.Token, "}" ) == 0 )
			{
				--IndentLevel;
				qualifiedToken.Qualifier = TokenQualifiers.CloseBracket;
			}
			else if ( string.Compare( qualifiedToken.Token, "" ) == 0 )
			{
				qualifiedToken.Qualifier = TokenQualifiers.BlankLine;
			}
			else
			{
				for ( var i = 0; i < expectedValues.Length; ++i )
				{
					if ( string.Compare( qualifiedToken.Token, expectedValues[ i ] ) == 0 )
					{
						qualifiedToken.Qualifier = TokenQualifiers.Recognized;
						break;
					}
				}
			}

			return qualifiedToken;
		}
	}
}
