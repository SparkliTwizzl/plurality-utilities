using System.Text;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using Petrichor.Common.Enums;
using Petrichor.Logging;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptTemplateParser
	{
		private StringTokenParser TokenParser = new StringTokenParser();


		public string[] ParseTemplatesFromData( string[] data, ref int i )
		{
			var taskMessage = "parsing templates from data";
			Log.TaskStarted( taskMessage );
			var templates = new List< string >();
			var expectedTokens = new string[] { };
			for ( ; i < data.Length; ++i )
			{
				var token = TokenParser.ParseToken( data[ i ], expectedTokens);
				var isParsingFinished = false;
				switch (token.Qualifier)
				{
					case StringTokenQualifiers.BlankLine:
						{
							break;
						}

					case StringTokenQualifiers.OpenBracket:
						{
							break;
						}

					case StringTokenQualifiers.CloseBracket:
						{
							if ( TokenParser.IndentLevel < 1 )
							{
								isParsingFinished = true;
							}
							break;
						}

					default:
						{
							templates.Add( ParseTemplateFromInputLine( token.Value ) );
							break;
						}
				}
				if ( isParsingFinished )
				{
					break;
				}
			}
			Log.TaskFinished( taskMessage );
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
						var errorMessage = "a template contained a trailing escape character ('\\') with no following character to escape";
						Log.Error( errorMessage );
						throw new EscapeCharacterMismatchException( errorMessage, ex );
					}
				}
				if ( ShortcutScriptTemplateMarkers.LookUpTable.TryGetValue( c, out var value ) )
				{
					template.Append( $"`{ value }`" );
				}
				else
				{
					template.Append( c );
				}
			}
			var result = template.ToString();
			Log.Info( $"parsed template: { result }" );
			return result;
		}
	}
}
