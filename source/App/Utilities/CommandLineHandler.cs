using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;
using System.CommandLine;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		public static async Task<int> ParseArguments( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				PrintHelpText();
				RuntimeHandler.WaitForUserAndExit();
			}

			var entriesFileOption = new Option< FileInfo? >( name: "--entries", description: "File with input entries data." );
			var templatesFileOption = new Option< FileInfo? >( name: "--templates", description: "File with input templates data." );
			var outputFileOption = new Option< FileInfo? >( name: "--output", description: "Output file for generated AutoHotkey script." );
			var logModeOption = new Option< string? >( name: "--logMode", description: "Logging mode to enable." );

			var rootCommand = new RootCommand( "Parse input files and generate an AutoHotkey script." );
			rootCommand.AddOption( entriesFileOption );
			rootCommand.AddOption( templatesFileOption );
			rootCommand.AddOption( outputFileOption );
			rootCommand.AddOption( logModeOption );

			rootCommand.SetHandler( ( file ) =>
					{
						StoreEntriesFilePath( file! );
					},
					entriesFileOption
				);
			rootCommand.SetHandler( ( file ) =>
					{
						StoreTemplatesFilePath( file! );
					},
					templatesFileOption
				);
			rootCommand.SetHandler( ( file ) =>
					{
						StoreOutputFilePath( file! );
					},
					outputFileOption
				);
			rootCommand.SetHandler( ( logMode ) =>
					{
						InitalizeLogging( logMode! );
					},
					logModeOption
				);

			return await rootCommand.InvokeAsync(arguments);
		}


		private static void InitalizeLogging( string logModeArgument )
		{
			switch ( logModeArgument )
			{
				case "consoleOnly":
					{
						Log.EnableForConsoleOnly( ProjectDirectories.LogDirectory );
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
		}

		private static void PrintHelpText()
		{
			Console.WriteLine("HELP:");
			Console.WriteLine("Pass input entries file path with \"--entries [file path]\".");
			Console.WriteLine("Pass input templates file path with \"--templates [file path]\".");
			Console.WriteLine("Pass output script file path with \"--output [file path]\".");
			Console.WriteLine("Pass logging mode to enable with \"--output [console/file/all]\".");
		}

		private static void StoreEntriesFilePath( FileInfo file )
		{
			RuntimeHandler.EntriesFilePath = file.FullName;
		}

		private static void StoreTemplatesFilePath( FileInfo file )
		{
			RuntimeHandler.TemplatesFilePath = file.FullName;
		}

		private static void StoreOutputFilePath( FileInfo file )
		{
			RuntimeHandler.OutputFilePath = file.FullName;
		}


	}
}
