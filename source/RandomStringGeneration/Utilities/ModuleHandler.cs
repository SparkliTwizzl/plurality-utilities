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
		private readonly struct TerminalCommandOptions
		{
			public static Option<string> AllowedCharacters => new(
				name: Commands.Options.AllowedCharacters,
				description: "Set of characters that random strings will be generated from." );

			public static Option<string> LogMode => new(
				name: Common.Syntax.Commands.Options.LogMode,
				description: "Logging mode to enable. See documentation for available modes." );

			public static Option<string> LogFile => new(
				name: Common.Syntax.Commands.Options.LogFile,
				description: "Path to generate log file at. If not provided, a default filepath will be used." );

			public static Option<string> OutputFile => new(
				name: Common.Syntax.Commands.Options.OutputFile,
				description: "Path to generate output file at. If not provided, a default file path will be used." );

			public static Option<string> StringCount => new(
				name: Commands.Options.StringCount,
				description: "Number of random strings to generate." );

			public static Option<string> StringLength => new(
				name: Commands.Options.StringLength,
				description: "Length of random strings." );
		}


		public static Command CreateTerminalCommand()
		{
			MetadataHandler.RegisterCommandOptions( Commands.Options.LookUpTable, Tokens.CommandOptionLookUpTable );

			var moduleCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Generate a list of random text strings." )
				{
					TerminalCommandOptions.AllowedCharacters,
					TerminalCommandOptions.LogMode,
					TerminalCommandOptions.LogFile,
					TerminalCommandOptions.OutputFile,
					TerminalCommandOptions.StringCount,
					TerminalCommandOptions.StringLength,
				};

			moduleCommand.SetHandler( async ( allowedCharacters, stringCount, stringLength, outputFilePath, logMode, logFile ) =>
				{
					MetadataHandler.CommandToRun = new()
					{
						Name = Commands.ModuleCommand,
						Options = new()
						{
							{ Commands.Options.AllowedCharacters, allowedCharacters },
							{ Common.Syntax.Commands.Options.LogMode, logMode },
							{ Common.Syntax.Commands.Options.LogFile, logFile },
							{ Common.Syntax.Commands.Options.OutputFile, outputFilePath },
							{ Commands.Options.StringCount, stringCount },
							{ Commands.Options.StringLength, stringLength },
						},
					};

					await MetadataHandler.InitalizeLogging( logMode, logFile );
					Log.WriteBufferToFile();
					Log.DisableBuffering();
				},
				TerminalCommandOptions.AllowedCharacters, TerminalCommandOptions.LogMode, TerminalCommandOptions.LogFile, TerminalCommandOptions.OutputFile, TerminalCommandOptions.StringCount, TerminalCommandOptions.StringLength );

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
			var outputFilePath = command.Options[ Common.Syntax.Commands.Options.OutputFile ];
			new StringGenerator().Generate( input, outputFilePath );
		}

		private static InputData GetInputDataFromCommand( ModuleCommand command )
		{
			var hasAllowedCharacters = command.Options.TryGetValue( Commands.Options.AllowedCharacters, out var allowedCharactersArgument );
			var isAllowedCharactersValid = allowedCharactersArgument is not null && allowedCharactersArgument.Length > 0;
			if ( !isAllowedCharactersValid )
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.AllowedCharacters}\" argument must be a string of 1 or more characters." ) );
			}

			var hasStringCount = command.Options.TryGetValue( Commands.Options.StringCount, out var stringCountArgument );
			var isStringCountOptionAnInt = int.TryParse( stringCountArgument, out var stringCount );
			var isStringCountOptionValid = isStringCountOptionAnInt && stringCount > 0;
			if ( hasStringCount && !isStringCountOptionValid )
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringCount}\" argument must be a positive integer." ) );
			}

			var hasStringLength = command.Options.TryGetValue( Commands.Options.StringLength, out var stringLengthArgument );
			var isStringLengthOptionAnInt = int.TryParse( stringLengthArgument, out var stringLength );
			var isStringLengthOptionValid = isStringLengthOptionAnInt && stringLength > 0;
			if ( hasStringLength && !isStringLengthOptionValid )
			{
				ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringLength}\" argument must be a positive integer." ) );
			}

			return new InputData()
			{
				AllowedCharacters = hasAllowedCharacters ? allowedCharactersArgument! : Commands.Options.AllowedCharactersDefaultValue,
				StringCount = hasStringCount ? stringCount : Commands.Options.StringCountDefaultValue,
				StringLength = hasStringLength ? stringLength : Commands.Options.StringLengthDefaultValue,
			};
		}
	}
}
