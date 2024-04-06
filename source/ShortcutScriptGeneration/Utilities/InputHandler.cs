using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputHandler
	{
		private ITokenBodyParser<List<ScriptEntry>> EntryListParser { get; set; }
		private ITokenBodyParser<ScriptInput> FileParser { get; set; }
		private ITokenBodyParser<ScriptModuleOptions> ModuleOptionsParser { get; set; }
		private IShortcutProcessor ShortcutProcessor { get; set; }
		private ITokenBodyParser<ScriptInput> ShortcutListParser { get; set; }


		public InputHandler( ITokenBodyParser<ScriptModuleOptions> moduleOptionsParser, ITokenBodyParser<List<ScriptEntry>> entryListParser, ITokenBodyParser<ScriptInput> shortcutListParser, IShortcutProcessor shortcutGenerator )
		{
			EntryListParser = entryListParser;
			ModuleOptionsParser = moduleOptionsParser;
			ShortcutProcessor = shortcutGenerator;
			ShortcutListParser = shortcutListParser;
			FileParser = CreateParser();
		}


		public ScriptInput ParseFileData( IndexedString[] regionData ) => FileParser.Parse( regionData );


		private TokenBodyParser<ScriptInput> CreateParser()
		{
			var entryListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result.Entries = EntryListParser.Parse( dataTrimmedToToken ).ToArray();
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: EntryListParser.LinesParsed );
			};

			var moduleOptionsTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result.ModuleOptions = ModuleOptionsParser.Parse( dataTrimmedToToken );
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: ModuleOptionsParser.LinesParsed );
			};

			var shortcutListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result = ShortcutListParser.Parse( dataTrimmedToToken, result );
				return new ProcessedRegionData<ScriptInput>( value: result, bodySize: ShortcutListParser.LinesParsed );
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

			return new TokenBodyParser<ScriptInput>( parserDescriptor );
		}
	}
}
