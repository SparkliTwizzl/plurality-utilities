using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.RandomStringGeneration.Containers;
using Petrichor.RandomStringGeneration.Syntax;
using System.CommandLine;


namespace Petrichor.RandomStringGeneration.Utilities
{
	public static class ModuleHandler
	{
		public static Command CreateCLICommand()
		{
			var allowedCharactersOption = new Option<string>(
				name: Commands.AllowedCharactersOption,
				description: "Set of characters that random strings will be generated from." );

			var stringCountOption = new Option<string>(
				name: Commands.StringCountOption,
				description: "Number of random strings to generate." );

			var stringLengthOption = new Option<string>(
				name: Commands.StringLengthOption,
				description: "Length of random strings." );

			var outputFileOption = new Option<string>(
				name: Common.Syntax.Commands.OutputFileOption,
				description: "Path to generate output file at. If not provided, a default file path will be used." );

			var logModeOption = new Option<string>(
				name: Common.Syntax.Commands.LogModeOption,
				description: "Logging mode to enable. Options are: [none | consoleOnly | fileOnly | all]." );

			var logFileOption = new Option<string>(
				name: Common.Syntax.Commands.LogFileOption,
				description: "Path to generate log file at. If not provided, a default filepath will be used." );

			var moduleCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Generate a list of random text strings." )
				{
					allowedCharactersOption,
					stringCountOption,
					stringLengthOption,
					outputFileOption,
					logModeOption,
					logFileOption,
				};

			moduleCommand.SetHandler( async ( allowedCharacters, stringCount, stringLength, outputFilePath, logMode, logFile ) =>
				{
					MetadataHandler.CommandToRun = new()
					{
						Name = Commands.ModuleCommand,
						Options = new()
						{
							{ Commands.AllowedCharactersOption, allowedCharacters },
							{ Commands.StringCountOption, stringCount },
							{ Commands.StringLengthOption, stringLength },
							{ Common.Syntax.Commands.OutputFileOption, outputFilePath },
							{ Common.Syntax.Commands.LogModeOption, logMode },
							{ Common.Syntax.Commands.LogFileOption, logFile },
						},
					};

					await MetadataHandler.InitalizeLogging( logMode, logFile );
					Log.WriteBufferToFile();
					Log.DisableBuffering();
				},
				allowedCharactersOption, stringCountOption, stringLengthOption, outputFileOption, logModeOption, logFileOption );

			return moduleCommand;
		}

		public static void ExecuteCommand( ModuleCommand command )
		{
			Log.Important( "Generating random strings..." );
			GenerateRandomStrings( command );
			Log.Important( "Generated random strings successfully." );
		}


		private static void GenerateRandomStrings( ModuleCommand command )
		{
			var input = GetInputDataFromCommand( command );
			var outputFilePath = command.Options[ Common.Syntax.Commands.OutputFileOption ];
			new StringGenerator().Generate( input, outputFilePath );
		}

		private static InputData GetInputDataFromCommand( ModuleCommand command )
		{
			var hasAllowedCharacters = command.Options.TryGetValue( Commands.AllowedCharactersOption, out var allowedCharactersArgument );
			var isAllowedCharactersValid = allowedCharactersArgument is not null && allowedCharactersArgument.Length > 0;
			if (!isAllowedCharactersValid)
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.AllowedCharactersOption}\" argument must be a string of 1 or more characters." ) );
			}

			var hasStringCount = command.Options.TryGetValue( Commands.StringCountOption, out var stringCountArgument );
			var isStringCountOptionAnInt = int.TryParse( stringCountArgument, out var stringCount );
			var isStringCountOptionValid = isStringCountOptionAnInt && stringCount > 0;
			if ( hasStringCount && !isStringCountOptionValid )
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.StringCountOption}\" argument must be a positive integer." ) );
			}

			var hasStringLength = command.Options.TryGetValue( Commands.StringLengthOption, out var stringLengthArgument );
			var isStringLengthOptionAnInt = int.TryParse( stringLengthArgument, out var stringLength );
			var isStringLengthOptionValid = isStringLengthOptionAnInt && stringLength > 0;
			if ( hasStringLength && !isStringLengthOptionValid )
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.StringLengthOption}\" argument must be a positive integer." ) );
			}

			return new InputData()
			{
				AllowedCharacters = hasAllowedCharacters ? allowedCharactersArgument! : Commands.AllowedCharactersDefaultValue,
				StringCount = hasStringCount ? stringCount : Commands.StringCountDefaultValue,
				StringLength = hasStringLength ? stringLength : Commands.StringLengthDefaultValue,
			};
		}
	}
}
