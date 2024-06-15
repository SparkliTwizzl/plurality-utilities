using Petrichor.Common.Containers;
using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public class InputFileHandler
	{
		private const string DefaultInputDirectory = @".\";
		private const string DefaultInputFileName =
#if DEBUG
			@".\test\input\integration\generateTextShortcutScript_cli.petrichor";
#else
			"input.petrichor";
#endif

		private ITokenBodyParser<List<IndexedString>> FileRegionParser { get; set; }
		private ITokenBodyParser<List<IndexedString>> MetadataRegionParser { get; set; }


		public ModuleCommand ActiveCommand { get; private set; } = ModuleCommand.None;


		public InputFileHandler( ITokenBodyParser<List<IndexedString>> metadataRegionParser, ModuleCommand? command = null )
		{
			ActiveCommand = command ?? ModuleCommand.None;
			MetadataRegionParser = metadataRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public List<IndexedString> ParseRegionData( IndexedString[] regionData ) => FileRegionParser.Parse( regionData );

		public List<IndexedString> ProcessFile( string file )
		{
			var filePathHandler = new FilePathHandler( DefaultInputDirectory, DefaultInputFileName );
			filePathHandler.SetFile( file );
			var filePath = filePathHandler.FilePath;

			var taskMessage = $"Read input file \"{filePath}\"";
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
			var regionData = IndexedString.IndexRawStrings( fileData.ToArray() );
			var result = ParseRegionData( regionData.ToArray() );

			Log.Finish( taskMessage );
			return result;
		}


		private TokenBodyParser<List<IndexedString>> CreateRegionParser()
		{
			var metadataTokenHandler = ( IndexedString[] regionData, int regionStartIndex, List<IndexedString> result ) =>
				{
					var dataTrimmedToToken = regionData[ regionStartIndex.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToToken );
					var remainingData = dataTrimmedToToken[ MetadataRegionParser.LinesParsed.. ].ToList();
					FileRegionParser.CancelParsing();
					return new ProcessedRegionData<List<IndexedString>>( value: remainingData, bodySize: MetadataRegionParser.LinesParsed );
				};

			var parserDescriptor = new DataRegionParserDescriptor<List<IndexedString>>()
			{
				RegionToken = new()
				{
					Key = "input-file-header",
				},
				TokenHandlers = new()
				{
					{ Syntax.Tokens.Metadata, metadataTokenHandler },
				},
			};

			var parser = new TokenBodyParser<List<IndexedString>>( parserDescriptor );
			return parser;
		}
	}
}
