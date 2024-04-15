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
		public static Command CreateTerminalCommand()
		{
			MetadataHandler.RegisterCommandOptions( Commands.Options.LookUpTable, Tokens.CommandOptionLookUpTable );

			var allowedCharactersOption = new Option<string>(
				name: Commands.Options.AllowedCharacters,
				description: "Set of characters that random strings will be generated from.",
				parseArgument: result =>
				{
					if ( !result.Tokens.Any() )
					{
						return Commands.Options.Defaults.AllowedCharactersValue;
					}

					var argument = result.Tokens[ 0 ].Value;

					if ( argument.Length < 1 )
					{
						result.ErrorMessage = $"Command option \"{Commands.Options.AllowedCharacters}\" argument must be a string of 1 or more characters.";
						ExceptionLogger.LogAndThrow( new CommandException( result.ErrorMessage ) );
						return string.Empty;
					}

					return argument;
				} );

			var outputFileOption = new Option<string>(
				name: Common.Syntax.Commands.Options.OutputFile,
				description: "Path to generate output file at. If not provided, a default file path will be used." );

			var stringCountOption = new Option<int>(
				name: Commands.Options.StringCount,
				description: "Number of random strings to generate.",
				parseArgument: result =>
				{
					if ( !result.Tokens.Any() )
					{
						return Commands.Options.Defaults.StringCountValue;
					}

					if ( !int.TryParse( result.Tokens[ 0 ].Value, out var argument ) || argument < 1 )
					{
						ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringCount}\" argument must be a positive integer." ) );
						return 0;
					}

					return argument;
				} );

			var stringLengthOption = new Option<int>(
				name: Commands.Options.StringLength,
				description: "Length of random strings.",
				parseArgument: result =>
				{
					if ( !result.Tokens.Any() )
					{
						return Commands.Options.Defaults.StringLengthValue;
					}

					if ( !int.TryParse( result.Tokens[ 0 ].Value, out var argument ) || argument < 1 )
					{
						ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringLength}\" argument must be a positive integer." ) );
						return 0;
					}

					return argument;
				} );

			var moduleCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Generate a list of random text strings." )
				{
					allowedCharactersOption,
					outputFileOption,
					stringCountOption,
					stringLengthOption,
				};

			moduleCommand.SetHandler( ( allowedCharacters, outputFilePath, stringCount, stringLength ) =>
				{
					MetadataHandler.CommandToRun = new()
					{
						Name = Commands.ModuleCommand,
						Options = new()
						{
							{ Commands.Options.AllowedCharacters, allowedCharacters },
							{ Common.Syntax.Commands.Options.OutputFile, outputFilePath },
							{ Commands.Options.StringCount, stringCount.ToString() },
							{ Commands.Options.StringLength, stringLength.ToString() },
						},
					};

					Log.WriteBufferToFile();
					Log.DisableBuffering();
				},
				allowedCharactersOption, outputFileOption, stringCountOption, stringLengthOption );

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
			var hasAllowedCharacters = command.Options.TryGetValue( Commands.Options.StringCount, out var allowedCharacters );
			var hasStringCount = command.Options.TryGetValue( Commands.Options.StringCount, out var stringCount );
			var hasStringLength = command.Options.TryGetValue( Commands.Options.StringLength, out var stringLength );
			return new InputData()
			{
				AllowedCharacters = hasAllowedCharacters ? allowedCharacters! : Commands.Options.Defaults.AllowedCharactersValue,
				StringCount = hasStringCount ? int.Parse( stringCount! ) : Commands.Options.Defaults.StringCountValue,
				StringLength = hasStringLength ? int.Parse( stringLength! ) : Commands.Options.Defaults.StringLengthValue,
			};
		}
	}
}
