using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.RandomStringGeneration.Containers;
using Petrichor.RandomStringGeneration.Syntax;
using System.CommandLine;


namespace Petrichor.RandomStringGeneration.Utilities
{
	/// <summary>
	/// Provides methods to handle random string generation commands.
	/// </summary>
	public static class ModuleHandler
	{
		/// <summary>
		/// Creates a terminal command for generating random strings.
		/// </summary>
		/// <returns>A <see cref="Command"/> configured for random string generation.</returns>
		public static Command CreateTerminalCommand()
		{
			MetadataHandler.RegisterCommandOptions(Commands.CommandParameterMap, TokenPrototypes.CommandTokenMap);

			var randomStringCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Generate a list of random text strings.")
			{
				TerminalOptions.AllowedCharacters,
				Common.Utilities.TerminalOptions.OutputFile,
				TerminalOptions.StringCount,
				TerminalOptions.StringLength,
			};

			randomStringCommand.SetHandler((allowedCharacters, autoExit, logFile, logMode, outputFile, stringCount, stringLength) =>
			{
				Common.Utilities.TerminalOptions.IsAutoExitEnabled = autoExit;
				MetadataHandler.CommandToRun = new()
				{
					Name = Commands.ModuleCommand,
					Options = new()
					{
						{ Commands.Parameters.AllowedCharacters, allowedCharacters },
						{ Common.Syntax.Commands.Parameters.AutoExit, autoExit.ToString() },
						{ Common.Syntax.Commands.Parameters.LogFile, logFile },
						{ Common.Syntax.Commands.Parameters.LogMode, logMode },
						{ Common.Syntax.Commands.Parameters.OutputFile, outputFile },
						{ Commands.Parameters.StringCount, stringCount.ToString() },
						{ Commands.Parameters.StringLength, stringLength.ToString() },
					},
				};

				MetadataHandler.InitializeLogging(logMode, logFile);
				Logger.WriteBufferToFile();
				Logger.DisableBuffering();
			},
			TerminalOptions.AllowedCharacters,
			Common.Utilities.TerminalOptions.AutoExit,
			Common.Utilities.TerminalOptions.LogFile,
			Common.Utilities.TerminalOptions.LogMode,
			Common.Utilities.TerminalOptions.OutputFile,
			TerminalOptions.StringCount,
			TerminalOptions.StringLength);

			return randomStringCommand;
		}

		/// <summary>
		/// Executes the random string generation command.
		/// </summary>
		/// <param name="command">The command containing the parameters for random string generation.</param>
		public static void ExecuteCommand(ModuleCommand command)
		{
			Logger.Important("Generating random strings...");
			GenerateRandomStrings(command);
			Logger.Important("Generated random strings successfully.");
		}


		/// <summary>
		/// Generates random strings based on the provided command and writes them to a file.
		/// </summary>
		/// <param name="command">The command containing the parameters for random string generation.</param>
		private static void GenerateRandomStrings(ModuleCommand command)
		{
			var inputData = ExtractInputDataFromCommand(command);
			var outputFile = command.Options[Common.Syntax.Commands.Parameters.OutputFile];
			new RandomStringGenerator().GenerateRandomStringFile(inputData, outputFile);
		}

		/// <summary>
		/// Extracts input data from the provided command.
		/// </summary>
		/// <param name="command">The command containing the parameters for random string generation.</param>
		/// <returns>An <see cref="RandomStringInput"/> object populated with the command arguments.</returns>
		private static RandomStringInput ExtractInputDataFromCommand(ModuleCommand command)
		{
			var hasAllowedCharacters = command.Options.TryGetValue(Commands.Parameters.StringCount, out var allowedCharacters);
			var hasStringCount = command.Options.TryGetValue(Commands.Parameters.StringCount, out var stringCount);
			var hasStringLength = command.Options.TryGetValue(Commands.Parameters.StringLength, out var stringLength);
			return new RandomStringInput()
			{
				AllowedCharacters = hasAllowedCharacters ? allowedCharacters! : Commands.Arguments.AllowedCharactersDefault,
				StringCount = hasStringCount ? int.Parse(stringCount!) : Commands.Arguments.StringCountDefault,
				StringLength = hasStringLength ? int.Parse(stringLength!) : Commands.Arguments.StringLengthDefault,
			};
		}
	}
}
