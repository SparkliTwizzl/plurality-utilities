using Petrichor.Common.Containers;
using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public class InputHandler
	{
		private const string DefaultInputDirectory = @".\";
		private const string DefaultInputFileName = "input.petrichor";

		private IDataRegionParser<List<IndexedString>> FileRegionParser { get; set; }
		private IDataRegionParser<List<IndexedString>> MetadataRegionParser { get; set; }


		public ModuleCommand ActiveCommand { get; private set; } = ModuleCommand.None;


		public InputHandler( IDataRegionParser<List<IndexedString>> metadataRegionParser, ModuleCommand? command = null )
		{
			ActiveCommand = command ?? ModuleCommand.None;
			MetadataRegionParser = metadataRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public List<IndexedString> ParseRegionData( IndexedString[] regionData ) => FileRegionParser.Parse( regionData );

		public List<IndexedString> ProcessFile( string file )
		{
			var directory = Path.GetDirectoryName( file ) ?? DefaultInputDirectory;
			var fileName = Path.GetFileName( file ) ?? DefaultInputFileName;
			var filePath = Path.Combine( directory, fileName );

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
			var regionData = IndexedString.IndexStringArray( fileData.ToArray() );
			var result = ParseRegionData( regionData.ToArray() );

			Log.Finish( taskMessage );
			return result;
		}


		private DataRegionParser<List<IndexedString>> CreateRegionParser()
		{
			var metadataTokenHandler = ( IndexedString[] regionData, int regionStartIndex, List<IndexedString> result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					var remainingData = dataTrimmedToRegion[ MetadataRegionParser.LinesParsed.. ].ToList();
					FileRegionParser.CancelParsing();
					return new ProcessedRegionData<List<IndexedString>>()
					{
						BodySize = MetadataRegionParser.LinesParsed,
						Value = remainingData,
					};
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

			var parser = new DataRegionParser<List<IndexedString>>( parserDescriptor );
			return parser;
		}
	}
}
