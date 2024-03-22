using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputHandler
	{
		private IDataRegionParser<List<ScriptEntry>> EntryListRegionParser { get; set; }
		private IDataRegionParser<ScriptInput> FileRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IDataRegionParser<ScriptModuleOptions> ModuleOptionsRegionParser { get; set; }
		private IDataRegionParser<List<string>> TemplateListRegionParser { get; set; }


		public InputHandler( IDataRegionParser<ScriptModuleOptions> moduleOptionsRegionParser, IDataRegionParser<List<ScriptEntry>> entryListRegionParser, IDataRegionParser<List<string>> templateListRegionParser, IMacroGenerator macroGenerator )
		{
			EntryListRegionParser = entryListRegionParser;
			MacroGenerator = macroGenerator;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplateListRegionParser = templateListRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public ScriptInput ParseRegionData( IndexedString[] regionData ) => FileRegionParser.Parse( regionData );


		private DataRegionParser<ScriptInput> CreateRegionParser()
		{
			var entryListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					result.Entries = EntryListRegionParser.Parse( dataTrimmedToRegion ).ToArray();
					return new ProcessedRegionData<ScriptInput>()
					{
						BodySize = EntryListRegionParser.LinesParsed,
						Value = result,
					};
				};

			var moduleOptionsTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
					{
						var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
						result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
						return new ProcessedRegionData<ScriptInput>()
						{
							BodySize = ModuleOptionsRegionParser.LinesParsed,
							Value = result,
						};
					};

			var templateListTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
					{
						var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
						result.Templates = TemplateListRegionParser.Parse( dataTrimmedToRegion ).ToArray();
						return new ProcessedRegionData<ScriptInput>()
						{
							BodySize = TemplateListRegionParser.LinesParsed,
							Value = result,
						};
					};

			var postParseHandler = ( ScriptInput result ) =>
				{
					result.Macros = MacroGenerator.Generate( result );
					return result;
				};


			var parserDescriptor = new DataRegionParserDescriptor<ScriptInput>()
			{
				RegionToken = new()
				{
					Key = "input-file-data",
				},
				TokenHandlers = new()
				{
					{ Syntax.Tokens.EntryList, entryListTokenHandler },
					{ Syntax.Tokens.ModuleOptions, moduleOptionsTokenHandler },
					{ Syntax.Tokens.TemplateList, templateListTokenHandler },
				},
				PostParseHandler = postParseHandler,
			};

			return new DataRegionParser<ScriptInput>( parserDescriptor );
		}
	}
}
