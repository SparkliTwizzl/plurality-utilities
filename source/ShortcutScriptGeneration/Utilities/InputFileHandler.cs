using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileHandler
	{
		private const string DefaultInputDirectory = @".\";
		private const string DefaultInputFileName = "input.petrichor";

		private IRegionParser<List<ScriptEntry>> EntriesRegionParser { get; set; }
		private IRegionParser<ScriptInput> FileRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser<IndexedString> MetadataRegionParser { get; set; }
		private IRegionParser<ScriptModuleOptions> ModuleOptionsRegionParser { get; set; }
		private IRegionParser<List<string>> TemplatesRegionParser { get; set; }


		private Func<IndexedString[], int, ScriptInput, RegionData<ScriptInput>> EntriesTokenHandler =>
			( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					result.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion ).ToArray();
					return new RegionData<ScriptInput>()
					{
						BodySize = EntriesRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func<IndexedString[], int, ScriptInput, RegionData<ScriptInput>> MetadataTokenHandler =>
			( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					return new RegionData<ScriptInput>()
					{
						BodySize = MetadataRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func<IndexedString[], int, ScriptInput, RegionData<ScriptInput>> ModuleOptionsTokenHandler =>
			( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
					return new RegionData<ScriptInput>()
					{
						BodySize = ModuleOptionsRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func<ScriptInput, ScriptInput> PostParseHandler => ( ScriptInput result ) =>
		{
			result.Macros = MacroGenerator.Generate( result );
			return result;
		};

		private Func<IndexedString[], int, ScriptInput, RegionData<ScriptInput>> TemplatesTokenHandler =>
			( IndexedString[] regionData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					result.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion ).ToArray();
					return new RegionData<ScriptInput>()
					{
						BodySize = TemplatesRegionParser.LinesParsed,
						Value = result,
					};
				};


		public InputFileHandler( IRegionParser<IndexedString> metadataRegionParser, IRegionParser<ScriptModuleOptions> moduleOptionsRegionParser, IRegionParser<List<ScriptEntry>> entriesRegionParser, IRegionParser<List<string>> templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;
			FileRegionParser = CreateRegionParser();
		}


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
			var result = FileRegionParser.Parse( regionData.ToArray() );

			Log.Finish( taskMessage );
			return result;
		}


		private RegionParser<ScriptInput> CreateRegionParser()
		{
			var parserDescriptor = new RegionParserDescriptor<ScriptInput>()
			{
				RegionName = "input-file-data",
				TokenHandlers = new()
				{
					{ Syntax.TokenNames.EntriesRegion, EntriesTokenHandler },
					{ Common.Syntax.TokenNames.MetadataRegion, MetadataTokenHandler },
					{ Syntax.TokenNames.ModuleOptionsRegion, ModuleOptionsTokenHandler },
					{ Syntax.TokenNames.TemplatesRegion, TemplatesTokenHandler },
				},
				PostParseHandler = PostParseHandler,
				MaxAllowedTokenInstances = new()
				{
					{ Syntax.TokenNames.EntriesRegion, Info.TokenMetadata.MaxEntriesRegions },
					{ Common.Syntax.TokenNames.MetadataRegion, Common.Info.TokenMetadata.MaxMetadataRegions },
					{ Syntax.TokenNames.ModuleOptionsRegion, Info.TokenMetadata.MaxModuleOptionsRegions },
					{ Syntax.TokenNames.TemplatesRegion, Info.TokenMetadata.MaxTemplatesRegions },
				},
				MinRequiredTokenInstances = new()
				{
					{ Syntax.TokenNames.EntriesRegion, Info.TokenMetadata.MinEntriesRegions },
					{ Common.Syntax.TokenNames.MetadataRegion, Common.Info.TokenMetadata.MinMetadataRegions },
					{ Syntax.TokenNames.ModuleOptionsRegion, Info.TokenMetadata.MinModuleOptionsRegions },
					{ Syntax.TokenNames.TemplatesRegion, Info.TokenMetadata.MinTemplatesRegions },
				},
			};

			return new RegionParser<ScriptInput>( parserDescriptor );
		}
	}
}
