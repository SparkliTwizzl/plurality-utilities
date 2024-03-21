using Petrichor.Common.Containers;
using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public class InputHandler
	{
		private const string DefaultInputDirectory = @".\";
		private const string DefaultInputFileName = "input.petrichor";

		private IDataRegionParser<IndexedString> FileRegionParser { get; set; }
		private IDataRegionParser<IndexedString> MetadataRegionParser { get; set; }
		private IDataRegionParser<IndexedString> CommandOptionsRegionParser { get; set; }


		private Func<IndexedString[], int, IndexedString, ProcessedRegionData<IndexedString>> MetadataHandler =>
			( IndexedString[] regionData, int regionStartIndex, IndexedString result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					return new ProcessedRegionData<IndexedString>()
					{
						BodySize = MetadataRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func<IndexedString[], int, IndexedString, ProcessedRegionData<IndexedString>> CommandHandler =>
			( IndexedString[] regionData, int regionStartIndex, IndexedString result ) =>
				{
					var dataTrimmedToRegion = regionData[ regionStartIndex.. ];
					result = CommandOptionsRegionParser.Parse( dataTrimmedToRegion );
					return new ProcessedRegionData<IndexedString>()
					{
						BodySize = CommandOptionsRegionParser.LinesParsed,
						Value = result,
					};
				};


		public InputHandler( IDataRegionParser<IndexedString> metadataRegionParser, IDataRegionParser<IndexedString> commandOptionsRegionParser )
		{
			MetadataRegionParser = metadataRegionParser;
			CommandOptionsRegionParser = commandOptionsRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public IndexedString ProcessFile( string file )
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


		private DataRegionParser<IndexedString> CreateRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<IndexedString>()
			{
				RegionToken = new()
				{
					Key = "input-file-data",
				},
				TokenHandlers = new()
				{
					{ Syntax.Tokens.Metadata, MetadataHandler },
					{ Syntax.Tokens.Command, CommandHandler },
				},
			};

			return new DataRegionParser<IndexedString>( parserDescriptor );
		}
	}
}
