using Petrichor.Common.Containers;
using Petrichor.Common.Info;
using Petrichor.Common.Syntax;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;


namespace Petrichor.App.Utilities
{
	/// <summary>
	/// Static class for handling command-line arguments and invoking module commands.
	/// </summary>
	public static class CommandLineManager
	{
		/// <summary>
		/// Argument representing the input file path.
		/// Defaults to "input.petrichor" in the input directory.
		/// </summary>
		private static Argument<string> InputFileArgument { get; } = new(
			name: Commands.Parameters.InputFile,
			description: "Path to input file.",
			getDefaultValue: () => Path.Combine(ProjectDirectories.InputDirectory, "input.petrichor"));

		/// <summary>
		/// Parses command-line arguments and invokes the root command asynchronously.
		/// If the arguments are invalid, prints usage information to the console.
		/// </summary>
		/// <param name="arguments">Command-line arguments.</param>
		/// <returns>A Task representing the asynchronous operation, returning a <see cref="ModuleCommand"/> containing the command to run.</returns>
		public static async Task<ModuleCommand> HandleCommandLineInput(string[] arguments)
		{
			if (arguments.Length < 1)
			{
				Console.WriteLine($"Run with {Syntax.Commands.HelpOption} to see usage.");
				return ModuleCommand.None;
			}

			var rootCommand = ConfigureRootCommand();
			_ = await rootCommand.InvokeAsync(arguments);
			return MetadataHandler.CommandToRun;
		}

		/// <summary>
		/// Creates the root command and configures terminal commands, arguments, and options.
		/// </summary>
		/// <returns>A configured <see cref="RootCommand"/> representing the application's root command.</returns>
		private static RootCommand ConfigureRootCommand()
		{
			MetadataHandler.RegisterCommandOptions(Commands.CommandParameterMap, TokenPrototypes.CommandTokenMap);

			var rootCommand = new RootCommand(description: "Command line app with miscellaneous utilities.")
			{
				InputFileArgument,
			};

			rootCommand.Handler = CommandHandler.Create((ParseResult parseResult) =>
			{
				var inputFile = string.Empty;
				try
				{
					TerminalOptions.IsAutoExitEnabled = parseResult.HasOption(TerminalOptions.AutoExit);

					var inputHandler = new InputFileHandler(metadataTokenParser: MetadataHandler.CreateMetadataTokenParser());
					inputFile = parseResult.GetValueForArgument(InputFileArgument) ?? string.Empty;
					var data = inputHandler.ProcessFile(inputFile).ToArray();
					MetadataHandler.CommandToRun.Data = data;

					_ = MetadataHandler.CommandToRun.Options.TryGetValue(Commands.Parameters.LogMode, out var logMode);
					_ = MetadataHandler.CommandToRun.Options.TryGetValue(Commands.Parameters.LogFile, out var logFile);
					MetadataHandler.InitializeLogging(
						logMode ?? Commands.Arguments.LogModeAll,
						logFile ?? TerminalOptions.DefaultLogFileName);

					Logger.WriteBufferToFile();
					Logger.DisableBuffering();
				}
				catch (Exception exception)
				{
					Logger.Error($"Failed to parse input file \"{inputFile}\": {exception.Message}");
					Logger.Important("If you file a bug report, please include the input and log files to help developers reproduce the issue.");
					MetadataHandler.CommandToRun = ModuleCommand.None;
				}
			});

			rootCommand.AddGlobalOption(TerminalOptions.AutoExit);
			rootCommand.AddGlobalOption(TerminalOptions.LogFile);
			rootCommand.AddGlobalOption(TerminalOptions.LogMode);

			rootCommand.AddCommand(RandomStringGeneration.Utilities.ModuleHandler.CreateTerminalCommand());
			rootCommand.AddCommand(ShortcutScriptGeneration.Utilities.ModuleHandler.CreateTerminalCommand());

			return rootCommand;
		}
	}
}
