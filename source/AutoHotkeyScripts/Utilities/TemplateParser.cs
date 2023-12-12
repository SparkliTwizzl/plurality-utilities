using System.Text;
using Petrichor.AutoHotkeyScripts.Exceptions;
using Petrichor.AutoHotkeyScripts.LookUpTables;
using Petrichor.Common.Enums;
using Petrichor.Logging;


namespace Petrichor.AutoHotkeyScripts.Utilities
{
	public class TemplateParser
	{
		private TokenParser TokenParser = new TokenParser();


		public string[] ParseTemplatesFromData( string[] data, ref int i )
		{
			Log.WriteLineTimestamped( "started parsing templates from data" );
			var templates = new List< string >();
			var expectedTokens = new string[] { };
			for ( ; i < data.Length; ++i )
			{
				var token = TokenParser.ParseToken( data[ i ], expectedTokens);
				var isParsingFinished = false;
				switch (token.Qualifier)
				{
					case TokenQualifiers.BlankLine:
						break;
					case TokenQualifiers.OpenBracket:
						break;
					case TokenQualifiers.CloseBracket:
						if ( TokenParser.IndentLevel < 1 )
						{
							isParsingFinished = true;
						}
						break;
					default:
						templates.Add( ParseTemplateFromInputLine( token.Value ) );
						break;
				}
				if ( isParsingFinished )
				{
					break;
				}
			}
			Log.WriteLineTimestamped( "finished parsing templates from data" );
			return templates.ToArray();
		}


		private string ParseTemplateFromInputLine( string input )
		{
			StringBuilder template = new StringBuilder();
			input = input.Trim();
			for ( int i = 0; i < input.Length; ++i )
			{
				var c = input[ i ];
				if ( c == '\\' )
				{
					try
					{
						template.Append( input[ i + 1 ] );
						++i;
						continue;
					}
					catch ( Exception ex )
					{
						var error = "a template contained a trailing escape character ('\\') with no following character to escape";
						Log.WriteLineTimestamped( $"error: { error }; { ex.Message }" );
						throw new EscapeCharacterMismatchException( error, ex );
					}
				}
				if ( TemplateMarkers.LookUpTable.TryGetValue( c, out var value ) )
				{
					template.Append( $"`{ value }`" );
				}
				else
				{
					template.Append( c );
				}
			}
			var result = template.ToString();
			Log.WriteLineTimestamped( $"parsed template: { result }" );
			return result;
		}
	}
}
