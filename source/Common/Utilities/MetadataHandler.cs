using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;
using System.Text;


namespace Petrichor.Common.Utilities
{
	public static class MetadataHandler
	{
		private static string DefaultLogDirectory => $@"{AppContext.BaseDirectory}\_log";
		private static string DefaultLogFileName => $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";


		public static ModuleCommand CommandToRun { get; set; } = ModuleCommand.None;


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
				var commandLineOption = Commands.LookUpTable[ token.Key ];
				result.Options.Add( commandLineOption, token.Value );
				return new ProcessedRegionData<ModuleCommand>( result );
			};

			var parserDescriptor = new DataRegionParserDescriptor<ModuleCommand>()
			{
				RegionToken = Syntax.Tokens.Command,
				TokenHandlers = new()
				{
					{ Syntax.Tokens.Command, commandTokenHandler },
					{ Syntax.Tokens.LogFile, commandOptionTokenHandler },
					{ Syntax.Tokens.LogMode, commandOptionTokenHandler },
					{ Syntax.Tokens.OutputFile, commandOptionTokenHandler },
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
				RegionToken = Syntax.Tokens.Metadata,
				TokenHandlers = new()
				{
					{ Syntax.Tokens.Command, commandTokenHandler },
					{ Syntax.Tokens.MinimumVersion, minimumVersionTokenHandler },
				},
			};

			return new TokenBodyParser<List<IndexedString>>( parserDescriptor );
		}

		public static async Task InitalizeLogging( string logModeArgument, string logFileArgument )
			=> await Task.Run( () =>
			{
				switch ( logModeArgument )
				{
					case Commands.LogModeOptionValueConsoleOnly:
					{
						Log.EnableLoggingToConsole();
						break;
					}

					case Commands.LogModeOptionValueFileOnly:
					{
						Log.EnableLoggingToFile();
						break;
					}

					case Commands.LogModeOptionValueAll:
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
					Console.WriteLine( $"Logging to console is disabled. To enable it, use command option \"{Commands.LogModeOption}\" with parameters \"{Commands.LogModeOptionValueConsoleOnly}\" or \"{Commands.LogModeOptionValueAll}\"." );
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
			ExceptionLogger.LogAndThrow( exception: new CommandException( $"A(n) \"{Syntax.Tokens.Metadata.Key}\" region contains a command \"{command.Name}\" which conflicts with active command \"{CommandToRun.Name}\"." ) );
		}
	}
}
