using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;
using Petrichor.RandomStringGeneration.Containers;
using Petrichor.RandomStringGeneration.Exceptions;
using System.Text;


namespace Petrichor.RandomStringGeneration.Utilities
{
	/// <summary>
	/// Generates random strings and writes them to an output file.
	/// </summary>
	public class RandomStringGenerator
	{
		private static string DefaultOutputDirectory => ProjectDirectories.OutputDirectory;
		private static string DefaultOutputFileName => $"output.{OutputFileExtension}";
		private static string OutputFileExtension => "txt";

		private RandomStringInput InputData { get; set; } = new();
		private string OutputFilePath { get; set; } = string.Empty;
		private int TotalLinesWritten { get; set; } = 0;


		/// <summary>
		/// Generates random strings based on the provided input data and writes them to the specified output file.
		/// </summary>
		/// <param name="input">The input data containing parameters for random string generation.</param>
		/// <param name="outputFile">The path to the output file where the random strings will be written.</param>
		/// <exception cref="StringGenerationException">Thrown when the output file cannot be generated.</exception>
		public void GenerateRandomStringFile(RandomStringInput input, string outputFile)
		{
			InputData = input;
			var filePathHandler = new FilePathHandler(DefaultOutputDirectory, DefaultOutputFileName);
			filePathHandler.SetFile(outputFile);
			var filePath = Path.ChangeExtension(filePathHandler.FilePath, OutputFileExtension);

			var taskMessage = $"Generate output file \"{filePath}\"";
			Logger.Start(taskMessage);

			try
			{
				var directory = Path.GetDirectoryName(filePath);
				_ = Directory.CreateDirectory(directory!);
				var file = File.Create(filePath);
				var randomStrings = GenerateRandomStringList();
				WriteLinesToFile(file, randomStrings, " (Random string list)");
				file.Close();
			}
			catch (Exception exception)
			{
				ExceptionLogger.LogAndThrow(new StringGenerationException($"Failed to generate output file \"{OutputFilePath}\".", exception));
			}

			Logger.Info($"Wrote {TotalLinesWritten} total lines to output file.");
			Logger.Finish(taskMessage);
		}


		private string GenerateRandomString()
		{
			var random = new Random();
			var builder = new StringBuilder();
			int randomChar;
			for (var i = 0; i < InputData.StringLength; ++i)
			{
				randomChar = random.Next() % InputData.AllowedCharacters.Length;
				_ = builder.Append(InputData.AllowedCharacters[randomChar]);
			}
			return builder.ToString();
		}

		private string[] GenerateRandomStringList()
		{
			var stringList = new List<string>();
			for (var i = 0; i < InputData.StringCount; ++i)
			{
				stringList.Add(GenerateRandomString());
			}
			return stringList.ToArray();
		}

		private void WriteLineToFile(FileStream file, string line = "")
		{
			try
			{
				var bytes = Encoding.UTF8.GetBytes($"{line}\n");
				file.Write(bytes);
				++TotalLinesWritten;
			}
			catch (Exception exception)
			{
				ExceptionLogger.LogAndThrow(new FileLoadException("Failed to write line to output file.", exception));
			}
		}

		private void WriteLinesToFile(FileStream file, string[] lines, string message = "")
		{
			var linesWritten = 0;
			foreach (var line in lines)
			{
				WriteLineToFile(file, line);
				++linesWritten;
			}
			Logger.Info($"Wrote {linesWritten} lines to output file{message}.");
		}
	}
}
