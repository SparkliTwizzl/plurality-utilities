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
	public static class CommandLineHandler
	{
		private static Argument<string> InputFileArgument { get; } = new(
			name: Commands.Options.InputFile,
			description: "Path to input file.",
			getDefaultValue: () => Path.Combine( ProjectDirectories.InputDirectory, "input.petrichor" ) );


		public static async Task<ModuleCommand> ParseArguments( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				Console.WriteLine( $"Run with {Syntax.Commands.HelpOption} to see usage." );
				return ModuleCommand.None;
			}

			var rootCommand = CreateTerminalCommands();
			_ = await rootCommand.InvokeAsync( arguments );
			return MetadataHandler.CommandToRun;
		}


		private static RootCommand CreateTerminalCommands()
		{
			MetadataHandler.RegisterCommandOptions( Commands.Options.LookUpTable, Tokens.CommandOptionLookUpTable );

			var rootCommand = new RootCommand( description: "Command line app with miscellaneous utilities." )
				{
					InputFileArgument,
				};
			rootCommand.Handler = CommandHandler.Create( ( ParseResult parseResult ) =>
				{
					var inputFile = string.Empty;
					try
					{
						TerminalOptions.IsAutoExitEnabled = parseResult.HasOption( TerminalOptions.AutoExit );

						var inputHandler = new InputFileHandler( metadataRegionParser: MetadataHandler.CreateMetadataTokenParser() );
						inputFile = parseResult.GetValueForArgument( InputFileArgument ) ?? string.Empty;
						var data = inputHandler.ProcessFile( inputFile ).ToArray();
						MetadataHandler.CommandToRun.Data = data;

						_ = MetadataHandler.CommandToRun.Options.TryGetValue( Commands.Options.LogMode, out var logMode );
						_ = MetadataHandler.CommandToRun.Options.TryGetValue( Commands.Options.LogFile, out var logFile );
						MetadataHandler.InitializeLogging(
							logMode ?? Commands.Options.LogModeValueAll,
							logFile ?? TerminalOptions.DefaultLogFileName );
						Log.WriteBufferToFile();
						Log.DisableBuffering();
					}
					catch ( Exception exception )
					{
						Log.Error( $"Failed to parse input file \"{inputFile}\": {exception.Message}" );
						Log.Important( "If you file a bug report, please include the input and log files to help developers reproduce the issue." );
						MetadataHandler.CommandToRun = ModuleCommand.None;
					}
				} );

			rootCommand.AddGlobalOption( TerminalOptions.AutoExit );
			rootCommand.AddGlobalOption( TerminalOptions.LogFile );
			rootCommand.AddGlobalOption( TerminalOptions.LogMode );
			rootCommand.AddCommand( RandomStringGeneration.Utilities.ModuleHandler.CreateTerminalCommand() );
			rootCommand.AddCommand( ShortcutScriptGeneration.Utilities.ModuleHandler.CreateTerminalCommand() );
			return rootCommand;
		}
	}
}
