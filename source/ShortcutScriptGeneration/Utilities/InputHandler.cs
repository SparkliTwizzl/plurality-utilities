using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputHandler
	{
		private IDataRegionParser<List<ScriptEntry>> EntryListRegionParser { get; set; }
		private IDataRegionParser<ScriptInput> FileRegionParser { get; set; }
		private IDataRegionParser<ScriptModuleOptions> ModuleOptionsRegionParser { get; set; }
		private IShortcutProcessor ShortcutProcessor { get; set; }
		private IDataRegionParser<ScriptInput> ShortcutListRegionParser { get; set; }


		public InputHandler( IDataRegionParser<ScriptModuleOptions> moduleOptionsRegionParser, IDataRegionParser<List<ScriptEntry>> entryListRegionParser, IDataRegionParser<ScriptInput> shortcutListRegionParser, IShortcutProcessor shortcutGenerator )
		{
			EntryListRegionParser = entryListRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			ShortcutProcessor = shortcutGenerator;
			ShortcutListRegionParser = shortcutListRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public ScriptInput ParseRegionData( IndexedString[] regionData ) => FileRegionParser.Parse( regionData );


		private DataRegionParser<ScriptInput> CreateRegionParser()
		{
			var entryListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
				result.Entries = EntryListRegionParser.Parse( dataTrimmedToRegion ).ToArray();
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: EntryListRegionParser.LinesParsed );
			};

			var moduleOptionsTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
				result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: ModuleOptionsRegionParser.LinesParsed );
			};

			var shortcutListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
				result = ShortcutListRegionParser.Parse( dataTrimmedToRegion );
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: ShortcutListRegionParser.LinesParsed );
			};

			var parserDescriptor = new DataRegionParserDescriptor<ScriptInput>()
			{
				RegionToken = new()
				{
					Key = "text-shortcuts-script-input",
				},
				TokenHandlers = new()
				{
					{ Syntax.Tokens.EntryList, entryListTokenHandler },
					{ Syntax.Tokens.ModuleOptions, moduleOptionsTokenHandler },
					{ Syntax.Tokens.ShortcutList, shortcutListTokenHandler },
				},
				PostParseHandler = ( ScriptInput result ) =>
				{
					result = ShortcutProcessor.ProcessAndStoreShortcuts( result );
					return result;
				},
			};

			return new DataRegionParser<ScriptInput>( parserDescriptor );
		}
	}
}
