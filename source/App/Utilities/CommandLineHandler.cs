using Petrichor.App.Syntax;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.CommandLine;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
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
			var inputFileOption = new Option<string>(
				name: CommandOptions.ShortcutScriptOptionInputFile,
				description: "Path to input file." );

			var rootCommand = new RootCommand( description: "Command line app with miscellaneous utilities." )
			{
				inputFileOption,
			};

			rootCommand.SetHandler( async ( inputFilePath ) =>
					{
						try
						{
							CommandToRun.Data = new InputHandler( CreateMetadataRegionParser() )
								.ProcessFile( inputFilePath )
								.ToArray();
							var logMode = CommandToRun.Options[ CommandOptions.ShortcutScriptOptionLogMode ];
							var logFile = CommandToRun.Options[ CommandOptions.ShortcutScriptOptionLogFile ];
							await InitalizeLogging( logMode, logFile );
						}
						catch ( Exception exception )
						{
							Log.Error( $"Failed to parse input file \"{inputFilePath}\": {exception.Message}" );
							Log.Important( "If you file a bug report, please include the input and log files to help developers reproduce the issue." );
							CommandToRun = ModuleCommand.None;
						}
					},
					inputFileOption );
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
						var inputHandler = new InputHandler( CreateMetadataRegionParser() );
						CommandToRun.Data = inputHandler.ProcessFile( inputFilePath ).ToArray();
					},
					inputFileOption, outputFileOption, logModeOption, logFileOption );

			return shortcutScriptCommand;
		}

		private static DataRegionParser<ModuleCommand> CreateCommandRegionParser()
		{
			var commandTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ModuleCommand result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Name = token.Value;
				return new ProcessedRegionData<ModuleCommand>()
				{
					Value = result,
				};
			};

			var commandOptionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ModuleCommand result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Options.Add( token.Key, token.Value );
				return new ProcessedRegionData<ModuleCommand>()
				{
					Value = result,
				};
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
			};

			return new DataRegionParser<ModuleCommand>( parserDescriptor );
		}

		private static DataRegionParser<List<IndexedString>> CreateMetadataRegionParser()
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
				var dataTrimmedToRegion = regionData[ tokenStartIndex.. ];
				CommandToRun = ModuleCommand.Some;
				var command = commandRegionParser.Parse( dataTrimmedToRegion );
				RejectConflictingModuleCommands( command );
				CommandToRun = command;
				return new ProcessedRegionData<List<IndexedString>>()
				{
					BodySize = commandRegionParser.LinesParsed - 1,
				};
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

			return new DataRegionParser<List<IndexedString>>( parserDescriptor );
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
						Log.EnableForConsole();
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentFileOnly:
					{
						Log.EnableForFile();
						break;
					}

					case CommandOptions.ShortcutScriptLogModeArgumentAll:
					{
						Log.EnableForAll();
						break;
					}

					default:
					{
						Log.Disable();
						break;
					}
				}

				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( $"Logging to console is disabled. To enable it, use command option \"{CommandOptions.ShortcutScriptOptionLogMode}\" with parameters \"{CommandOptions.ShortcutScriptLogModeArgumentConsoleOnly}\" or \"{CommandOptions.ShortcutScriptLogModeArgumentAll}\"." );
				}

				if ( Log.IsLoggingToFileEnabled )
				{
					Log.CreateLogFile( logFileArgument );
				}

				Console.WriteLine();
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
			=> CommandToRun.Data = new InputHandler( CreateMetadataRegionParser() ).ProcessFile( inputFile ).ToArray();
	}
}
