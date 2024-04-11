using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public static class MetadataHandler
	{
		private static string DefaultLogDirectory => $@"{AppContext.BaseDirectory}\_log";
		private static string DefaultLogFileName => $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";

		private static Dictionary<string, string> CommandOptionTerminalFlags { get; set; } = new();
		private static Dictionary<string, DataToken> CommandOptionTokens { get; set; } = new();


		public static ModuleCommand CommandToRun { get; set; } = ModuleCommand.None;


		public static void RegisterCommandOptions( Dictionary<string, string> terminalFlags, Dictionary<string, DataToken> tokens )
		{
			foreach ( var item in terminalFlags )
			{
				CommandOptionTerminalFlags.Add( item.Key, item.Value );
			}
			foreach ( var item in tokens )
			{
				CommandOptionTokens.Add( item.Key, item.Value );
			}
		}

		public static TokenBodyParser<ModuleCommand> CreateCommandTokenParser()
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
				var commandLineOption = CommandOptionTerminalFlags[ token.Key ];
				result.Options.Add( commandLineOption, token.Value );
				return new ProcessedRegionData<ModuleCommand>( result );
			};

			var parserDescriptor = new DataRegionParserDescriptor<ModuleCommand>()
			{
				RegionToken = Tokens.Command,
				TokenHandlers = new()
				{
					{ Tokens.Command, commandTokenHandler },
				},
			};

			var parser = new TokenBodyParser<ModuleCommand>( parserDescriptor );
			foreach ( var token in CommandOptionTokens.Values )
			{
				parser.AddTokenHandler( token, commandOptionTokenHandler );
			}
			return parser;
		}

		public static TokenBodyParser<List<IndexedString>> CreateMetadataTokenParser()
		{
			var commandRegionParser = CreateCommandTokenParser();

			var minimumVersionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<IndexedString> result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var version = token.Value;
				Info.AppVersion.RejectUnsupportedVersions( version: version, lineNumber: token.LineNumber );
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
				RegionToken = Tokens.Metadata,
				TokenHandlers = new()
				{
					{ Tokens.Command, commandTokenHandler },
					{ Tokens.MinimumVersion, minimumVersionTokenHandler },
				},
			};

			return new TokenBodyParser<List<IndexedString>>( parserDescriptor );
		}

		public static async Task InitalizeLogging( string logModeArgument, string logFileArgument )
			=> await Task.Run( () =>
			{
				switch ( logModeArgument )
				{
					case Commands.Options.LogModeValueConsoleOnly:
					{
						Log.EnableLoggingToConsole();
						break;
					}

					case Commands.Options.LogModeValueFileOnly:
					{
						Log.EnableLoggingToFile();
						break;
					}

					case Commands.Options.LogModeValueAll:
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
					Console.WriteLine( $"Logging to console is disabled. To enable it, use command option \"{Commands.Options.LogMode}\" with parameters \"{Commands.Options.LogModeValueConsoleOnly}\" or \"{Commands.Options.LogModeValueAll}\"." );
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
			ExceptionLogger.LogAndThrow( exception: new CommandException( $"A(n) \"{Tokens.Metadata.Key}\" region contains a command \"{command.Name}\" which conflicts with active command \"{CommandToRun.Name}\"." ) );
		}
	}
}
