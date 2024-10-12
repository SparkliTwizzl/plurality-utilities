using Petrichor.Common.Containers;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;


namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Handles the processing and parsing of input files.
	/// </summary>
	public class InputFileHandler
	{
		private const string DefaultInputDirectory =
#if DEBUG
			"test/input/";
#else
			"./";
#endif

		private const string DefaultInputFileName = "input.petrichor";

		private ITokenBodyParser<List<IndexedString>> FileBodyParser { get; set; }
		private ITokenBodyParser<List<IndexedString>> MetadataBodyParser { get; set; }


		/// <summary>
		/// The currently active module command to run on the input file's contents.
		/// </summary>
		public ModuleCommand ActiveCommand { get; private set; } = ModuleCommand.None;


		/// <summary>
		/// Initializes a new instance of the <see cref="InputFileHandler"/> class
		/// with the specified metadata parser and optional command.
		/// </summary>
		/// <param name="metadataTokenParser">Parser for the metadata token of the input file.</param>
		/// <param name="command">Optional command to set as active.</param>
		public InputFileHandler( ITokenBodyParser<List<IndexedString>> metadataTokenParser, ModuleCommand? command = null )
		{
			ActiveCommand = command ?? ModuleCommand.None;
			MetadataBodyParser = metadataTokenParser;
			FileBodyParser = CreateFileBodyParser();
		}


		/// <summary>
		/// Parses data from the provided array of indexed strings.
		/// </summary>
		/// <param name="fileBody">Array of indexed strings representing the input file's body.</param>
		/// <returns>A list of indexed strings parsed from the region data.</returns>
		public List<IndexedString> ParseFileBody( IndexedString[] fileBody ) => FileBodyParser.Parse( fileBody );

		/// <summary>
		/// Processes the input file, reads its contents, and parses the region data.
		/// </summary>
		/// <param name="file">The path of the input file to process.</param>
		/// <returns>A list of indexed strings parsed from the input file.</returns>
		/// <exception cref="FileNotFoundException">Thrown when the specified input file does not exist.</exception>
		/// <exception cref="IOException">Thrown when an I/O error occurs while reading the file.</exception>
		public List<IndexedString> ProcessFile( string file )
		{
			var filePathHandler = new FilePathHandler( DefaultInputDirectory, DefaultInputFileName );
			filePathHandler.SetFile( file );
			var filePath = filePathHandler.FilePath;

			var taskMessage = $"Read input file \"{filePath}\"";
			Logger.Start( taskMessage );

			var fileData = new List<string>();
			try
			{
				fileData = File.ReadAllLines( filePath ).ToList();
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new FileNotFoundException( $"Input file was not found (\"{filePath}\").", exception ) );
			}

			if ( fileData.Count < 1 )
			{
				return new List<IndexedString>();
			}

			var fileBody = IndexedString.IndexRawStrings( fileData.ToArray() );
			var result = ParseFileBody( fileBody.ToArray() );

			Logger.Finish( taskMessage );
			return result;
		}


		/// <summary>
		/// Creates a parser for the input file body.
		/// </summary>
		/// <returns>An instance of a token body parser for the input file.</returns>
		private TokenBodyParser<List<IndexedString>> CreateFileBodyParser()
		{
			var metadataTokenHandler = ( IndexedString[] bodyData, int regionStartIndex, List<IndexedString> result ) =>
			{
				var dataTrimmedToToken = bodyData[ regionStartIndex.. ];
				_ = MetadataBodyParser.Parse( dataTrimmedToToken );
				var remainingData = dataTrimmedToToken[ MetadataBodyParser.TotalLinesParsed.. ].ToList();
				FileBodyParser.CancelParsing();
				return new ProcessedTokenData<List<IndexedString>>( value: remainingData, bodySize: MetadataBodyParser.TotalLinesParsed );
			};

			var parserDescriptor = new TokenParseDescriptor<List<IndexedString>>()
			{
				TokenPrototype = new()
				{
					Key = "input-file-header",
				},
				SubTokenHandlers = new()
				{
					{ Syntax.TokenPrototypes.Metadata, metadataTokenHandler },
				},
			};

			var parser = new TokenBodyParser<List<IndexedString>>( parserDescriptor );
			return parser;
		}
	}
}
