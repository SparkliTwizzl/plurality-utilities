using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
	{
		private IRegionParser< List< ScriptEntry >> EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser< StringWrapper > MetadataRegionParser { get; set; }
		private IRegionParser< ScriptModuleOptions > ModuleOptionsRegionParser { get; set; }
		private IRegionParser< List< string >> TemplatesRegionParser { get; set; }
		private IRegionParser< ScriptInput > FileRegionParser { get; set; }


		public InputFileParser( IRegionParser< StringWrapper > metadataRegionParser, IRegionParser< ScriptModuleOptions > moduleOptionsRegionParser, IRegionParser< List< ScriptEntry > > entriesRegionParser, IRegionParser< List< string > > templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;

			var EntriesRegionHandler = ( string[] data, int regionStartIndex, ScriptInput scriptInput ) =>
			{
				var dataTrimmedToRegion = data[ regionStartIndex.. ];
				var regionData = EntriesRegionParser.Parse( dataTrimmedToRegion );
				scriptInput.Entries = regionData.Value.ToArray();
				return new DataToken< ScriptInput >()
				{
					TokenLinesLength = EntriesRegionParser.LinesParsed,
					Value = scriptInput,
				};
			};

			var MetadataRegionHandler = ( string[] data, int regionStartIndex, ScriptInput scriptInput ) =>
			{
				var dataTrimmedToRegion = data[ regionStartIndex.. ];
				var regionData = MetadataRegionParser.Parse( dataTrimmedToRegion );
				scriptInput.Metadata = regionData.Value.ToArray();
				return new DataToken< ScriptInput >()
				{
					TokenLinesLength = regionData.TokenLinesLength,
					Value = scriptInput,
				};

				/*
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					i += MetadataRegionParser.LinesParsed;
				 */
			};

			var fileRegionTokenHandlers = new Dictionary< string, Func< StringToken, ScriptInput, DataToken< ScriptInput >>>()
			{
				{ Syntax.TokenNames.EntriesRegion, EntriesRegionHandler },
				{ Common.Syntax.TokenNames.MetadataRegion,  },
				{ Syntax.TokenNames.ModuleOptionsRegion,  },
				{ Syntax.TokenNames.TemplatesRegion,  },
			};
			FileRegionParser = new RegionParser< ScriptInput >( "Input file", 1, fileRegionTokenHandlers );
		}


		public ScriptInput Parse( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
			Log.TaskStart( taskMessage );

			var fileContents = File.ReadAllLines( filePath );
			var data = FileRegionParser.Parse( fileContents );
			var input = data.Value;
			input.Macros = MacroGenerator.Generate( input );
			Log.TaskFinish( taskMessage );
			return input;
		}
	}
}
