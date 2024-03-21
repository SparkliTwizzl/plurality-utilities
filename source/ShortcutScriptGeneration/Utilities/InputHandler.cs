using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputHandler
	{
		private const string DefaultInputDirectory = @".\";
		private const string DefaultInputFileName = "input.petrichor";

		private IDataRegionParser<List<ScriptEntry>> EntryListRegionParser { get; set; }
		private IDataRegionParser<ScriptInput> FileRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IDataRegionParser<IndexedString> MetadataRegionParser { get; set; }
		private IDataRegionParser<ScriptModuleOptions> ModuleOptionsRegionParser { get; set; }
		private IDataRegionParser<List<string>> TemplateListRegionParser { get; set; }


		public InputHandler( IDataRegionParser<IndexedString> metadataRegionParser, IDataRegionParser<ScriptModuleOptions> moduleOptionsRegionParser, IDataRegionParser<List<ScriptEntry>> entryListRegionParser, IDataRegionParser<List<string>> templateListRegionParser, IMacroGenerator macroGenerator )
		{
			EntryListRegionParser = entryListRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplateListRegionParser = templateListRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public ScriptInput ParseRegions( IndexedString[] regionData ) => FileRegionParser.Parse( regionData );

		public ScriptInput ProcessFile( string file )
		{
			var directory = Path.GetDirectoryName( file ) ?? DefaultInputDirectory;
			var fileName = Path.GetFileName( file ) ?? DefaultInputFileName;
			var filePath = Path.Combine( directory, fileName );

			var taskMessage = $"Parse input file \"{filePath}\"";
			Log.Start( taskMessage );

			var fileData = new List<string>();
			try
			{
				fileData = File.ReadAllLines( filePath ).ToList();
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new FileNotFoundException( $"Input file was not found (\"{filePath}\").", exception ) );
			}
			var regionData = IndexedString.IndexStringArray( fileData.ToArray() );
			var result = ParseRegions( regionData.ToArray() );

			Log.Finish( taskMessage );
			return result;
		}


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

			var metadataTokenHandler = ( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					return new ProcessedRegionData<ScriptInput>()
					{
						BodySize = MetadataRegionParser.LinesParsed,
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
					{ Common.Syntax.Tokens.Metadata, metadataTokenHandler },
					{ Syntax.Tokens.ModuleOptions, moduleOptionsTokenHandler },
					{ Syntax.Tokens.TemplateList, templateListTokenHandler },
				},
				PostParseHandler = postParseHandler,
			};

			return new DataRegionParser<ScriptInput>( parserDescriptor );
		}
	}
}
