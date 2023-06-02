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
			if ( string.Compare( qualifiedToken.Token, "" ) == 0 )
			{
				qualifiedToken.Qualifier = TokenQualifiers.BlankLine;
			}
			else if ( string.Compare( qualifiedToken.Token, "{" ) == 0 )
			{
				++IndentLevel;
				qualifiedToken.Qualifier = TokenQualifiers.OpenBracket;
			}
			else if ( string.Compare( qualifiedToken.Token, "}" ) == 0 )
			{
				--IndentLevel;
				qualifiedToken.Qualifier = TokenQualifiers.CloseBracket;
			}
			else
			{
				foreach ( var value in expectedValues )
				{
					if ( string.Compare( qualifiedToken.Token, value ) == 0 )
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
