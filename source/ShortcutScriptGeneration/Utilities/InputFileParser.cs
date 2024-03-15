using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IMetadataRegionParser MetadataRegionParser { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }
		private IRegionParser< ScriptInput > FileRegionParser { get; set; }


		public InputFileParser( IMetadataRegionParser metadataRegionParser, IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;

			var EntriesRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ regionStartIndex.. ];
				result.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					LineCount = EntriesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var MetadataRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ regionStartIndex.. ];
				_ = MetadataRegionParser.Parse( dataTrimmedToRegion ) ;
				return new RegionData< ScriptInput >()
				{
					LineCount = MetadataRegionParser.LinesParsed,
					Value = result,
				};
			};

			var ModuleOptionsRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ regionStartIndex.. ];
				result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					LineCount = ModuleOptionsRegionParser.LinesParsed,
					Value = result,
				};
			};

			var TemplatesRegionHandler = ( string[] fileData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = fileData[ regionStartIndex.. ];
				result.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion );
				return new RegionData< ScriptInput >()
				{
					LineCount = TemplatesRegionParser.LinesParsed,
					Value = result,
				};
			};

			var fileRegionTokenHandlers = new Dictionary< string, Func< string[], int, ScriptInput, RegionData< ScriptInput > > >()
			{
				{ Syntax.TokenNames.EntriesRegion, EntriesRegionHandler },
				{ Common.Syntax.TokenNames.MetadataRegion, MetadataRegionHandler },
				{ Syntax.TokenNames.ModuleOptionsRegion, ModuleOptionsRegionHandler },
				{ Syntax.TokenNames.TemplatesRegion, TemplatesRegionHandler },
			};
			FileRegionParser = new RegionParser< ScriptInput >( "Input file body", 1, fileRegionTokenHandlers );
		}


		public ScriptInput Parse( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
			Log.TaskStart( taskMessage );

			var fileData = File.ReadAllLines( filePath );
			var result = FileRegionParser.Parse( fileData );
			result.Macros = MacroGenerator.Generate( result );

			if ( MetadataRegionParser.RegionsParsed < 1 )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input files must contain a { Common.Syntax.TokenNames.MetadataRegion } region." ) );
			}

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
