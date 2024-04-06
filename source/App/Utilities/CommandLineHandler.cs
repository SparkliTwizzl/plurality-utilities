using Petrichor.App.Syntax;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.CommandLine;
using System.Text;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		private static string DefaultLogDirectory => $@"{AppContext.BaseDirectory}\_log";
		private static string DefaultLogFileName => $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";



		public static ModuleCommand CommandToRun { get; set; } = ModuleCommand.None;


		public static async Task<ModuleCommand> ParseArguments( string[] arguments )
		{
			if ( !WereArgumentsProvided( arguments ) )
			{
				return ModuleCommand.None;
			}

			var rootCommand = CreateCLICommands();
			_ = await rootCommand.InvokeAsync( arguments );
			return CommandToRun;
		}


		private static RootCommand CreateCLICommands()
		{
			var inputFileArgument = new Argument<string>(
				name: CommandOptions.ShortcutScriptOptionInputFile,
				description: "Path to input file." );

			var rootCommand = new RootCommand( description: "Command line app with miscellaneous utilities." )
			{
				inputFileArgument,
			};

			rootCommand.SetHandler( async ( inputFilePath ) =>
				{
					try
					{
						var inputHandler = new InputHandler( metadataRegionParser: CreateMetadataRegionParser() );
						var data = inputHandler.ProcessFile( inputFilePath ).ToArray();
						CommandToRun.Data = data;
						var logMode = CommandToRun.Options[ CommandOptions.ShortcutScriptOptionLogMode ];
						var logFile = CommandToRun.Options[ CommandOptions.ShortcutScriptOptionLogFile ];
						await InitalizeLogging( logMode, logFile );
						Log.WriteBufferToFile();
						Log.DisableBuffering();
					}
					catch ( Exception exception )
					{
						Log.Error( $"Failed to parse input file \"{inputFilePath}\": {exception.Message}" );
						Log.Important( "If you file a bug report, please include the input and log files to help developers reproduce the issue." );
						CommandToRun = ModuleCommand.None;
					}
				},
				inputFileArgument );

			rootCommand.AddCommand( CreateCLIShortcutScriptCommand() );
			return rootCommand;
		}

		private static Command CreateCLIShortcutScriptCommand()
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

			var shortcutScriptCommand = new Command(
				name: Commands.GenerateShortcutScript,
				description: "Parse input file and generate a text find-and-replace shortcut script." )
			{
				inputFileOption,
				outputFileOption,
				logModeOption,
				logFileOption,
			};

			shortcutScriptCommand.SetHandler( async ( inputFilePath, outputFilePath, logMode, logFile ) =>
				{
					CommandToRun = new()
					{
						Name = Commands.GenerateShortcutScript,
						Options = new()
						{
							{ CommandOptions.ShortcutScriptOptionInputFile, inputFilePath },
							{ CommandOptions.ShortcutScriptOptionOutputFile, outputFilePath },
							{ CommandOptions.ShortcutScriptOptionLogMode, logMode },
							{ CommandOptions.ShortcutScriptOptionLogFile, logFile },
						},
					};

					await InitalizeLogging( logMode, logFile );
					Log.WriteBufferToFile();
					Log.DisableBuffering();

					var inputHandler = new InputHandler( CreateMetadataRegionParser() );
					CommandToRun.Data = inputHandler.ProcessFile( inputFilePath ).ToArray();
				},
				inputFileOption, outputFileOption, logModeOption, logFileOption );

			return shortcutScriptCommand;
		}

		private static TokenBodyParser<ModuleCommand> CreateCommandRegionParser()
		{
			var commandTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ModuleCommand result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Name = token.Value;
				return new ProcessedRegionData<ModuleCommand>( result );
			};

			var commandOptionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ModuleCommand result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var commandLineOption = TokenKeysToCommandLineOptions.LookUpTable[ token.Key ];
				result.Options.Add( commandLineOption, token.Value );
				return new ProcessedRegionData<ModuleCommand>( result );
			};

			var parserDescriptor = new DataRegionParserDescriptor<ModuleCommand>()
			{
				RegionToken = Common.Syntax.Tokens.Command,
				TokenHandlers = new()
				{
					{ Common.Syntax.Tokens.Command, commandTokenHandler },
					{ Common.Syntax.Tokens.LogFile, commandOptionTokenHandler },
					{ Common.Syntax.Tokens.LogMode, commandOptionTokenHandler },
					{ Common.Syntax.Tokens.OutputFile, commandOptionTokenHandler },
				},
				PostParseHandler = ( ModuleCommand result ) =>
				{
					var optionListStringBuilder = new StringBuilder();
					foreach ( var option in result.Options )
					{
						_ = optionListStringBuilder.Append( $" {option.Key} {option.Value}" );
					}
					Log.Info( $"Command to run: \"{result.Name}{optionListStringBuilder}\"" );
					return result;
				},
			};

			return new TokenBodyParser<ModuleCommand>( parserDescriptor );
		}

		private static TokenBodyParser<List<IndexedString>> CreateMetadataRegionParser()
		{
			var commandRegionParser = CreateCommandRegionParser();

			var minimumVersionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<IndexedString> result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var version = token.Value;
				Common.Info.AppVersion.RejectUnsupportedVersions( version: version, lineNumber: token.LineNumber );
				return new ProcessedRegionData<List<IndexedString>>();
			};

			var commandTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<IndexedString> result ) =>
			{
				var dataTrimmedToToken = regionData[ tokenStartIndex.. ];
				CommandToRun = ModuleCommand.Some;
				var command = commandRegionParser.Parse( dataTrimmedToToken );
				RejectConflictingModuleCommands( command );
				CommandToRun = command;
				return new ProcessedRegionData<List<IndexedString>>( value: result, bodySize: commandRegionParser.LinesParsed - 1 );
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<IndexedString>>()
			{
				RegionToken = Common.Syntax.Tokens.Metadata,
				TokenHandlers = new()
				{
					{ Common.Syntax.Tokens.Command, commandTokenHandler },
					{ Common.Syntax.Tokens.MinimumVersion, minimumVersionTokenHandler },
				},
			};

			return new TokenBodyParser<List<IndexedString>>( parserDescriptor );
		}

		private static bool WereArgumentsProvided( string[] arguments )
		{
			if ( arguments.Length < 1 )
			{
				Console.WriteLine( $"Run with {CommandOptions.DefaultCommandOptionHelp} to see usage." );
				return false;
			}

			return true;
		}

		private static async Task InitalizeLogging( string logModeArgument, string logFileArgument )
			=> await Task.Run( () =>
			{
				switch ( logModeArgument )
				{
					case CommandOptions.ShortcutScriptLogModeArgumentConsoleOnly:
					{
						Log.EnableLoggingToConsole();
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentFileOnly:
					{
						Log.EnableLoggingToFile();
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentAll:
					{
						Log.EnableAllLogDestinations();
						break;
					}

					default:
					{
						Log.DisableLogging();
						break;
					}
				}

				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( $"Logging to console is disabled. To enable it, use command option \"{CommandOptions.ShortcutScriptOptionLogMode}\" with parameters \"{CommandOptions.ShortcutScriptLogModeArgumentConsoleOnly}\" or \"{CommandOptions.ShortcutScriptLogModeArgumentAll}\"." );
				}

				if ( Log.IsLoggingToFileEnabled )
				{
					var filePathHandler = new FilePathHandler( DefaultLogDirectory, DefaultLogFileName );
					filePathHandler.SetFile( logFileArgument );
					Log.CreateLogFile( filePathHandler.FilePath );
				}
			} );

		private static void RejectConflictingModuleCommands( ModuleCommand command )
		{
			var isCurrentCommandSet = CommandToRun != ModuleCommand.None && CommandToRun != ModuleCommand.Some;
			var isNewCommandSet = command != ModuleCommand.None;
			if ( !isCurrentCommandSet || !isNewCommandSet )
			{
				return;
			}

			ExceptionLogger.LogAndThrow( exception: new CommandException( $"A(n) \"{Common.Syntax.Tokens.Metadata.Key}\" region contains a command \"{command.Name}\" which conflicts with active command \"{CommandToRun.Name}\"." ) );
		}

		public static void TryParseInputFile( string inputFile )
		{
			CommandToRun.Data = new InputHandler( CreateMetadataRegionParser() ).ProcessFile( inputFile ).ToArray();
			CommandToRun.Options.Add( CommandOptions.ShortcutScriptOptionInputFile, inputFile );
		}
	}
}
