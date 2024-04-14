using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.RandomStringGeneration.Containers;
using Petrichor.RandomStringGeneration.Exceptions;
using System.Text;


namespace Petrichor.RandomStringGeneration.Utilities
{
	public class StringGenerator
	{
		private static readonly string DefaultOutputDirectory = ProjectDirectories.OutputDirectory;
		private static readonly string DefaultOutputFileName = $"output.{OutputFileExtension}";
		private const string OutputFileExtension = "txt";

		private InputData Input { get; set; } = new();
		private string OutputFilePath { get; set; } = string.Empty;
		private int TotalLinesWritten { get; set; } = 0;


		public void Generate( InputData input, string outputFile )
		{
			Input = input;
			var filePathHandler = new FilePathHandler( DefaultOutputDirectory, DefaultOutputFileName );
			filePathHandler.SetFile( outputFile );
			var filePath = Path.ChangeExtension( filePathHandler.FilePath, OutputFileExtension );

			var taskMessage = $"Generate output file \"{filePath}\"";
			Log.Start( taskMessage );

			try
			{
				CreateOutputFile( filePath );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new StringGenerationException( $"Failed to generate output file \"{OutputFilePath}\".", exception ) );
			}

			Log.Info( $"Wrote {TotalLinesWritten} total lines to output file." );
			Log.Finish( taskMessage );
		}


		private void CreateOutputFile( string filePath )
		{
			var directory = Path.GetDirectoryName( filePath );
			_ = Directory.CreateDirectory( directory! );
			var file = File.Create( filePath );
			var randomStrings = GenerateRandomStringList();
			WriteLinesToFile( file, randomStrings, " (Random string list)" );
			file.Close();
		}

		private string GenerateRandomString()
		{
			var random = new Random();
			var builder = new StringBuilder();
			int randomChar;
			for ( var i = 0 ; i < Input.StringLength ; ++i )
			{
				randomChar = random.Next() % Input.AllowedCharacters.Length;
				_ = builder.Append( Input.AllowedCharacters[ randomChar ] );
			}
			return builder.ToString();
		}

		private string[] GenerateRandomStringList()
		{
			var stringList = new List<string>();
			for ( var i = 0 ; i < Input.StringCount ; ++i )
			{
				stringList.Add( GenerateRandomString() );
			}
			return stringList.ToArray();
		}

		private void WriteLineToFile( FileStream file, string line = "" )
		{
			try
			{
				var bytes = Encoding.UTF8.GetBytes( $"{line}\n" );
				file.Write( bytes );
				++TotalLinesWritten;
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new FileLoadException( "Failed to write line to output file.", exception ) );
			}
		}

		private void WriteLinesToFile( FileStream file, string[] lines, string message = "" )
		{
			var linesWritten = 0;
			foreach ( var line in lines )
			{
				WriteLineToFile( file, line );
				++linesWritten;
			}
			Log.Info( $"Wrote {linesWritten} lines to output file{message}." );
		}

	}
}
