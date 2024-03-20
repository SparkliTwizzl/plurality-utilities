using Petrichor.App.Enums;
using Petrichor.App.Syntax;
using Petrichor.Common.Info;
using Petrichor.Logging;
using System.CommandLine;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		public static Module ModuleToRun { get; private set; } = Module.None;


		public static async Task<Module> ParseArguments( string[] arguments )
		{
			ModuleToRun = HandleEmptyArguments( arguments );
			if ( ModuleToRun == Module.None )
			{
				return Module.None;
			}

			var rootCommand = CreateCommands();
			_ = await rootCommand.InvokeAsync( arguments );
			return ModuleToRun;
		}


		private static RootCommand CreateCommands()
		{
			var rootCommand = new RootCommand( "Command line app with miscellaneous utilities." );
			rootCommand.AddCommand( CreateShortcutScriptCommand() );
			return rootCommand;
		}

		private static Command CreateShortcutScriptCommand()
		{
			var inputFileOption = new Option<string>(
				name: CommandOptions.ShortcutScriptOptionInputFile,
				description: "Path to input file." );

			var outputFileOption = new Option<string>(
				name: CommandOptions.ShortcutScriptOptionOutputFile,
				description: "Path and filename to generate AutoHotkey script at." );

			var logModeOption = new Option<string>(
				name: CommandOptions.ShortcutScriptOptionLogMode,
				description: "Logging mode to enable. Options are consoleOnly, fileOnly, all." );

			var logFileOption = new Option<string>(
				name: CommandOptions.ShortcutScriptOptionLogFile,
				description: "Path to generate log file at. If not provided, a default filepath will be used." );

			var shortcutScriptCommand = new Command( Commands.ShortcutScript, "Parse input file and generate a text find-and-replace shortcut script." )
			{
				inputFileOption,
				outputFileOption,
				logModeOption,
				logFileOption,
			};

			shortcutScriptCommand.SetHandler( async ( inputFilePath, outputFilePath, logMode, logFile ) =>
					{
						RuntimeHandler.InputFilePath = inputFilePath;
						RuntimeHandler.OutputFilePath = outputFilePath;
						ModuleToRun = Module.ShortcutScriptGeneration;
						await InitalizeLogging( logMode, logFile );
					},
					inputFileOption, outputFileOption, logModeOption, logFileOption
				);

			return shortcutScriptCommand;
		}

		private static Module HandleEmptyArguments( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				Console.WriteLine( $"Run with {CommandOptions.DefaultCommandOptionHelp} to see usage." );
				return Module.None;
			}

			return Module.Some;
		}

		private static async Task InitalizeLogging( string logModeArgument, string logFileArgument )
			=> await Task.Run( () =>
			{
				switch ( logModeArgument )
				{
					case CommandOptions.ShortcutScriptLogModeArgumentConsoleOnly:
					{
						Log.EnableForConsoleOnly();
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentFileOnly:
					{
						Log.EnableForFileOnly( ProjectDirectories.LogDirectory );
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentAll:
					{
						Log.EnableForAll( ProjectDirectories.LogDirectory );
						break;
					}

					default:
					{
						Log.Disable();
						break;
					}
				}

				if ( Log.IsLoggingToFileEnabled )
				{
					Log.SetLogFile( logFileArgument );
				}
			} );
	}
}
