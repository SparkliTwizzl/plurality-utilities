using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;


namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Handles metadata-related parsing and logging initialization.
	/// </summary>
	public static class MetadataHandler
	{
		private static Dictionary<string, string> CommandParameterMap { get; set; } = new();
		private static Dictionary<string, DataToken> CommandTokenMap { get; set; } = new();


		/// <summary>
		/// Gets or sets the command to run, determined by parsed metadata.
		/// </summary>
		public static ModuleCommand CommandToRun { get; set; } = ModuleCommand.None;

		/// <summary>
		/// Registers command options to allow them to be parsed from Petrichor Script.
		/// </summary>
		/// <param name="commandParameterMap">A dictionary mapping token keys to command parameters.</param>
		/// <param name="commandTokenMap">A dictionary mapping token keys to token prototypes.</param>
		public static void RegisterCommandOptions(Dictionary<string, string> commandParameterMap, Dictionary<string, DataToken> commandTokenMap)
		{
			foreach (var item in commandParameterMap)
			{
				CommandParameterMap.Add(item.Key, item.Value);
			}
			foreach (var item in commandTokenMap)
			{
				CommandTokenMap.Add(item.Key, item.Value);
			}
		}

		/// <summary>
		/// Creates a parser to handle command tokens.
		/// </summary>
		/// <returns>A <see cref="TokenBodyParser{T}"/> which generates a <see cref="ModuleCommand"/>.</returns>
		public static TokenBodyParser<ModuleCommand> CreateCommandTokenParser()
		{
			var commandTokenHandler = (IndexedString[] bodyData, int tokenStartIndex, ModuleCommand result) =>
			{
				var token = new StringToken(bodyData[tokenStartIndex]);
				result.Name = token.TokenValue;
				return new ProcessedTokenData<ModuleCommand>(result);
			};

			var commandOptionTokenHandler = (IndexedString[] bodyData, int tokenStartIndex, ModuleCommand result) =>
			{
				var token = new StringToken(bodyData[tokenStartIndex]);
				var commandLineOption = CommandParameterMap[token.TokenKey];
				result.Options.Add(commandLineOption, token.TokenValue);
				return new ProcessedTokenData<ModuleCommand>(result);
			};

			var parserDescriptor = new TokenParseDescriptor<ModuleCommand>()
			{
				TokenPrototype = TokenPrototypes.Command,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Command, commandTokenHandler },
				},
			};

			var parser = new TokenBodyParser<ModuleCommand>(parserDescriptor);
			foreach (var token in CommandTokenMap.Values)
			{
				parser.AddTokenHandler(token, commandOptionTokenHandler);
			}
			return parser;
		}

		/// <summary>
		/// Creates a parser to handle metadata tokens.
		/// </summary>
		/// <returns>A <see cref="TokenBodyParser{T}"/> which generates an <see cref="IndexedString"/>.</returns>
		/// <exception cref="CommandException">Thrown when conflicting module commands are detected in metadata.</exception>
		/// <exception cref="System.Data.VersionNotFoundException">Thrown when a minimum version token contains an unsupported version number string.</exception>
		public static TokenBodyParser<List<IndexedString>> CreateMetadataTokenParser()
		{
			var commandRegionParser = CreateCommandTokenParser();

			var minimumVersionTokenHandler = (IndexedString[] bodyData, int tokenStartIndex, List<IndexedString> result) =>
			{
				var token = new StringToken(bodyData[tokenStartIndex]);
				var version = token.TokenValue;
				Info.AppVersion.RejectUnsupportedVersions(version: version, lineNumber: token.LineNumber);
				return new ProcessedTokenData<List<IndexedString>>();
			};

			var commandTokenHandler = (IndexedString[] bodyData, int tokenStartIndex, List<IndexedString> result) =>
			{
				var dataTrimmedToToken = bodyData[tokenStartIndex..];
				CommandToRun = ModuleCommand.Some;
				var command = commandRegionParser.Parse(dataTrimmedToToken);
				RejectConflictingModuleCommands(command);
				CommandToRun = command;
				return new ProcessedTokenData<List<IndexedString>>(value: result, bodySize: commandRegionParser.TotalLinesParsed - 1);
			};

			var tokenParseDescriptor = new TokenParseDescriptor<List<IndexedString>>()
			{
				TokenPrototype = TokenPrototypes.Metadata,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Command, commandTokenHandler },
					{ TokenPrototypes.MinimumVersion, minimumVersionTokenHandler },
				},
			};

			return new TokenBodyParser<List<IndexedString>>(tokenParseDescriptor);
		}

		/// <summary>
		/// Initializes logging based on the provided arguments.
		/// </summary>
		/// <param name="logModeArgument">The logging mode to enable.</param>
		/// <param name="logFileArgument">The log file name and/or path to store.</param>
		public static void InitializeLogging(string logModeArgument, string logFileArgument)
		{
			switch (logModeArgument)
			{
				case Commands.Arguments.LogModeConsoleOnly:
					Logger.EnableConsoleLogging();
					break;

				case Commands.Arguments.LogModeFileOnly:
					Logger.EnableFileLogging();
					break;

				case Commands.Arguments.LogModeAll:
					Logger.EnableAllLogDestinations();
					break;

				default:
					Logger.DisableLogging();
					break;
			}

			if (!Logger.IsConsoleLoggingEnabled)
			{
				Console.WriteLine($"Logging to console is disabled. To enable it, use command option \"{Commands.Parameters.LogMode}\" with argument \"{Commands.Arguments.LogModeConsoleOnly}\" or \"{Commands.Arguments.LogModeAll}\".");
			}

			if (Logger.IsFileLoggingEnabled)
			{
				var filePathHandler = new FilePathHandler(TerminalOptions.DefaultLogDirectory, TerminalOptions.DefaultLogFileName);
				filePathHandler.SetFile(logFileArgument);
				Logger.CreateLogFile(filePathHandler.FilePath);
			}
		}


		/// <summary>
		/// Validates and rejects conflicting module commands.
		/// </summary>
		/// <param name="command">The parsed module command to validate.</param>
		/// <exception cref="CommandException">Thrown if a conflicting module command is detected.</exception>
		private static void RejectConflictingModuleCommands(ModuleCommand command)
		{
			var isCurrentCommandSet = CommandToRun != ModuleCommand.None && CommandToRun != ModuleCommand.Some;
			var isNewCommandSet = command != ModuleCommand.None;
			if (!isCurrentCommandSet || !isNewCommandSet)
			{
				return;
			}
			ExceptionLogger.LogAndThrow(new CommandException($"A(n) \"{TokenPrototypes.Metadata.Key}\" region contains a command \"{command.Name}\" which conflicts with active command \"{CommandToRun.Name}\"."));
		}
	}
}
