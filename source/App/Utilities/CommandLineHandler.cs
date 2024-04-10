using Petrichor.Common.Containers;
using Petrichor.Common.Syntax;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.CommandLine;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		public static async Task<ModuleCommand> ParseArguments( string[] arguments )
		{
			if ( !WereArgumentsProvided( arguments ) )
			{
				return ModuleCommand.None;
			}

			var rootCommand = CreateCLICommands();
			_ = await rootCommand.InvokeAsync( arguments );
			return MetadataHandler.CommandToRun;
		}


		private static RootCommand CreateCLICommands()
		{
			var inputFileArgument = new Argument<string>(
				name: Commands.InputFileOption,
				description: "Path to input file." );

			var rootCommand = new RootCommand( description: "Command line app with miscellaneous utilities." )
				{
					inputFileArgument,
				};

			rootCommand.SetHandler( async ( inputFilePath ) =>
				{
					try
					{
						var inputHandler = new InputFileHandler( metadataRegionParser: MetadataHandler.CreateMetadataTokenParser() );
						var data = inputHandler.ProcessFile( inputFilePath ).ToArray();
						MetadataHandler.CommandToRun.Data = data;
						var logMode = MetadataHandler.CommandToRun.Options[ Commands.LogModeOption ];
						var logFile = MetadataHandler.CommandToRun.Options[ Commands.LogFileOption ];
						await MetadataHandler.InitalizeLogging( logMode, logFile );
						Log.WriteBufferToFile();
						Log.DisableBuffering();
					}
					catch ( Exception exception )
					{
						Log.Error( $"Failed to parse input file \"{inputFilePath}\": {exception.Message}" );
						Log.Important( "If you file a bug report, please include the input and log files to help developers reproduce the issue." );
						MetadataHandler.CommandToRun = ModuleCommand.None;
					}
				},
				inputFileArgument );

			rootCommand.AddCommand( RandomStringGeneration.Utilities.ModuleHandler.CreateCLICommand() );
			rootCommand.AddCommand( ShortcutScriptGeneration.Utilities.ModuleHandler.CreateCLICommand() );
			return rootCommand;
		}

		private static bool WereArgumentsProvided( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				Console.WriteLine( $"Run with {App.Syntax.Commands.HelpOption} to see usage." );
				return false;
			}

			return true;
		}

		public static void TryParseInputFile( string inputFile )
		{
			MetadataHandler.CommandToRun.Data = new InputFileHandler( MetadataHandler.CreateMetadataTokenParser() ).ProcessFile( inputFile ).ToArray();
			MetadataHandler.CommandToRun.Options.Add( Commands.InputFileOption, inputFile );
		}
	}
}
