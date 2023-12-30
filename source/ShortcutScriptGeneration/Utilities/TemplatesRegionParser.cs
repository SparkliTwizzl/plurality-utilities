using Petrichor.Common.Enums;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class TemplatesRegionParser : ITemplatesRegionParser
	{
		private StringTokenParser TokenParser { get; set; } = new();


		public string[] ParseTemplatesFromData( string[] data, ref int i )
		{
			var taskMessage = $"parsing {ShortcutScriptGenerationSyntax.TemplatesRegionTokenName} region data";
			Log.TaskStarted( taskMessage );
			var templates = new List<string>();
			var expectedTokens = Array.Empty<string>();
			for ( ; i < data.Length ; ++i )
			{
				var token = TokenParser.ParseToken( data[ i ], expectedTokens );
				var isParsingFinished = false;
				switch ( token.Qualifier )
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


		private static string ParseTemplateFromInputLine( string input )
		{
			var template = new StringBuilder();
			input = input.Trim();
			for ( var i = 0 ; i < input.Length ; ++i )
			{
				var c = input[ i ];
				if ( c == '\\' )
				{
					try
					{
						_ = template.Append( input[ i + 1 ] );
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
				_ = ShortcutScriptTemplateMarkers.LookUpTable.TryGetValue( c, out var value )
					? template.Append( $"`{value}`" )
					: template.Append( c );
			}
			var result = template.ToString();
			Log.Info( $"parsed template: {result}" );
			return result;
		}
	}
}
