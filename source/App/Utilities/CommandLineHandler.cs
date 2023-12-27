using Petrichor.Common.Info;
using Petrichor.Logging;
using System.CommandLine;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		public static async Task<int> ParseArguments( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				Console.WriteLine( "Run with --help to see usage." );
				RuntimeHandler.WaitForUserAndExit();
			}

			var inputFileOption = new Option<string>( name: "--input", description: "Path to input file with entries and templates data." );
			var outputFileOption = new Option<string>( name: "--output", description: "Path and filename to generate AutoHotkey script at." );
			var logModeOption = new Option<string>( name: "--logMode", description: "Logging mode to enable. Options are consoleOnly, fileOnly, all." );
			var logFileOption = new Option<string>( name: "--logFile", description: "Path to generate log file at. If not provided, a default filepath will be used." );

			var rootCommand = new RootCommand( "Command line app with miscellaneous utilities." );
			var generateAHKScriptCommand = new Command( "generateAHKShortcutScript", "Parse input files and generate an AutoHotkey script." )
			{
				inputFileOption,
				outputFileOption,
				logModeOption,
				logFileOption,
			};
			rootCommand.AddCommand( generateAHKScriptCommand );

			generateAHKScriptCommand.SetHandler( async ( inputFilePath, outputFilePath, logMode, logFile ) =>
					{
						RuntimeHandler.InputFilePath = inputFilePath;
						RuntimeHandler.OutputFilePath = outputFilePath;
						await InitalizeLogging( logMode, logFile );
					},
					inputFileOption, outputFileOption, logModeOption, logFileOption
				);

			return await rootCommand.InvokeAsync( arguments );
		}


		private static async Task InitalizeLogging( string logModeArgument, string logFileArgument )
			=> await Task.Run( () =>
			{
				switch ( logModeArgument )
				{
					case "consoleOnly":
					{
						Log.EnableForConsoleOnly();
						break;
					}

					case "fileOnly":
					{
						Log.EnableForFileOnly( ProjectDirectories.LogDirectory );
						break;
					}

					case "all":
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
