using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputHandler
	{
		private ITokenBodyParser<List<Entry>> EntryListParser { get; set; }
		private ITokenBodyParser<InputData> FileParser { get; set; }
		private ITokenBodyParser<ModuleOptionData> ModuleOptionsParser { get; set; }
		private IShortcutProcessor ShortcutProcessor { get; set; }
		private ITokenBodyParser<InputData> ShortcutListParser { get; set; }


		public InputHandler( ITokenBodyParser<ModuleOptionData> moduleOptionsParser, ITokenBodyParser<List<Entry>> entryListParser, ITokenBodyParser<InputData> shortcutListParser, IShortcutProcessor shortcutGenerator )
		{
			EntryListParser = entryListParser;
			ModuleOptionsParser = moduleOptionsParser;
			ShortcutProcessor = shortcutGenerator;
			ShortcutListParser = shortcutListParser;
			FileParser = CreateParser();
		}


		public InputData ParseFileData( IndexedString[] regionData ) => FileParser.Parse( regionData );


		private TokenBodyParser<InputData> CreateParser()
		{
			var entryListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, InputData result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result.Entries = EntryListParser.Parse( dataTrimmedToToken ).ToArray();
				return new ProcessedRegionData<InputData>( value: result, bodySize: EntryListParser.LinesParsed );
			};

			var moduleOptionsTokenHandler = ( IndexedString[] regionData, int regionStartIndex, InputData result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result.ModuleOptions = ModuleOptionsParser.Parse( dataTrimmedToToken );
				return new ProcessedRegionData<InputData>( value: result, bodySize: ModuleOptionsParser.LinesParsed );
			};

			var shortcutListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, InputData result ) =>
			{
				var dataTrimmedToToken = regionData[ regionStartIndex.. ];
				result = ShortcutListParser.Parse( dataTrimmedToToken, result );
				return new ProcessedRegionData<InputData>( value: result, bodySize: ShortcutListParser.LinesParsed );
			};

			var parserDescriptor = new DataRegionParserDescriptor<InputData>()
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
				PostParseHandler = ( InputData result ) =>
				{
					result = ShortcutProcessor.ProcessAndStoreShortcuts( result );
					return result;
				},
			};

			return new TokenBodyParser<InputData>( parserDescriptor );
		}
	}
}
