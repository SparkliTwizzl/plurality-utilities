using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser< StringWrapper > MetadataRegionParser { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }
		private IRegionParser< ScriptInput > FileRegionParser { get; set; }


		public InputFileParser( IRegionParser< StringWrapper > metadataRegionParser, IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;

			var entriesRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = EntriesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var metadataRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
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

			var moduleOptionsRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = ModuleOptionsRegionParser.LinesParsed,
					Value = result,
				};
			};

			var templatesRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
				result.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					BodySize = TemplatesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var fileRegionTokenHandlers = new Dictionary< string, Func< string[], int, ScriptInput, RegionData< ScriptInput > > >()
			{
				{ Syntax.TokenNames.EntriesRegion, entriesRegionHandler },
				{ Common.Syntax.TokenNames.MetadataRegion, metadataRegionHandler },
				{ Syntax.TokenNames.ModuleOptionsRegion, moduleOptionsRegionHandler },
				{ Syntax.TokenNames.TemplatesRegion, templatesRegionHandler },
			};

			var fileRegionParserDesc = new RegionParserDescriptor< ScriptInput >()
			{
				RegionName = "Input file body",
				MaxRegionsAllowed = 1,
				MinRegionsRequired = 1,
				TokenHandlers = fileRegionTokenHandlers,
			};
			FileRegionParser = new RegionParser< ScriptInput >( fileRegionParserDesc );
		}


		public ScriptInput Parse( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
			Log.TaskStart( taskMessage );

			var fileData = File.ReadAllLines( filePath );
			var result = FileRegionParser.Parse( fileData );
			result.Macros = MacroGenerator.Generate( result );

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
