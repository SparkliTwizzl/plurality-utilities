using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileHandler
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser< StringWrapper > MetadataRegionParser { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }
		private IRegionParser< ScriptInput > FileRegionParser { get; set; }


		public InputFileHandler( IRegionParser< StringWrapper > metadataRegionParser, IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;

			var entriesTokenHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = EntriesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var metadataTokenHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				var resultMessage = MetadataRegionParser.Parse( dataTrimmedToRegion ) ;
				Log.Important( resultMessage.ToString() );
				return new RegionData< ScriptInput >()
				{
					BodySize = MetadataRegionParser.LinesParsed,
					Value = result,
				};
			};

			var moduleOptionsTokenHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = ModuleOptionsRegionParser.LinesParsed,
					Value = result,
				};
			};

			var templatesTokenHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = TemplatesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var tokenHandlers = new Dictionary< string, Func< string[], int, ScriptInput, RegionData< ScriptInput > > >()
			{
				{ Syntax.TokenNames.EntriesRegion, entriesTokenHandler },
				{ Common.Syntax.TokenNames.MetadataRegion, metadataTokenHandler },
				{ Syntax.TokenNames.ModuleOptionsRegion, moduleOptionsTokenHandler },
				{ Syntax.TokenNames.TemplatesRegion, templatesTokenHandler },
			};

			var postParseHandler = ( ScriptInput result ) =>
			{
				if ( !MetadataRegionParser.HasParsedMinRequiredRegions )
				{
					ExceptionLogger.LogAndThrow( new FileRegionException( $"Input files must contain at least { MetadataRegionParser.MinRegionsRequired } { MetadataRegionParser.RegionName } regions" ) );
				}

				result.Macros = MacroGenerator.Generate( result );
				return result;
			};

			var parserDescriptor = new RegionParserDescriptor< ScriptInput >()
			{
				RegionName = "Input file body",
				MaxRegionsAllowed = 1,
				MinRegionsRequired = 1,
				TokenHandlers = tokenHandlers,
				PostParseHandler = postParseHandler,
			};
			FileRegionParser = new RegionParser< ScriptInput >( parserDescriptor );
		}


		public ScriptInput ProcessFile( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
			Log.TaskStart( taskMessage );

			var fileData = File.ReadAllLines( filePath );
			var result = FileRegionParser.Parse( fileData );

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
