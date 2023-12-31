using Petrichor.Common.Enums;
using Petrichor.Common.Exceptions;
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


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;


		public string[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {ShortcutScriptGenerationSyntax.TemplatesRegionTokenName}";
			Log.TaskStart( taskMessage );
			
			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} regions" );
			}

			var templates = new List<string>();
			var expectedTokens = Array.Empty<string>();
			for ( var i = 0; i < regionData.Length ; ++i )
			{
				var token = TokenParser.ParseToken( regionData[ i ], expectedTokens );
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
						templates.Add( ParseTemplateFromLine( token.Value ) );
						break;
					}
				}
				if ( isParsingFinished )
				{
					LinesParsed = i;
					break;
				}
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.Info( $"Parsed {templates.Count} templates" );
			Log.TaskFinish( taskMessage );
			return templates.ToArray();
		}


		private static string ParseTemplateFromLine( string line )
		{
			var template = new StringBuilder();
			line = line.Trim();
			for ( var i = 0 ; i < line.Length ; ++i )
			{
				var c = line[ i ];
				if ( c == '\\' )
				{
					try
					{
						_ = template.Append( line[ i + 1 ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						throw new EscapeCharacterMismatchException( "A template contained a trailing escape character ('\\') with no following character to escape", exception );
					}
				}
				_ = ShortcutScriptTemplateMarkers.LookUpTable.TryGetValue( c, out var value )
					? template.Append( $"`{value}`" )
					: template.Append( c );
			}
			return template.ToString();
		}
	}
}
