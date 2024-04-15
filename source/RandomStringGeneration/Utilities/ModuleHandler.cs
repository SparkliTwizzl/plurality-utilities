using Petrichor.Common.Containers;
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

			var moduleCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Generate a list of random text strings." )
				{
					TerminalOptions.AllowedCharacters,
					Common.Utilities.TerminalOptions.OutputFile,
					TerminalOptions.StringCount,
					TerminalOptions.StringLength,
				};

			moduleCommand.SetHandler( ( allowedCharacters, autoExit, logFile, logMode, outputFile, stringCount, stringLength ) =>
				{
					Common.Utilities.TerminalOptions.IsAutoExitEnabled = autoExit;
					MetadataHandler.CommandToRun = new()
					{
						Name = Commands.ModuleCommand,
						Options = new()
						{
							{ Commands.Options.AllowedCharacters, allowedCharacters },
							{ Common.Syntax.Commands.Options.AutoExit, autoExit.ToString() },
							{ Common.Syntax.Commands.Options.LogFile, logFile },
							{ Common.Syntax.Commands.Options.LogMode, logMode },
							{ Common.Syntax.Commands.Options.OutputFile, outputFile },
							{ Commands.Options.StringCount, stringCount.ToString() },
							{ Commands.Options.StringLength, stringLength.ToString() },
						},
					};

					Log.WriteBufferToFile();
					Log.DisableBuffering();
				},
				TerminalOptions.AllowedCharacters,
				Common.Utilities.TerminalOptions.AutoExit,
				Common.Utilities.TerminalOptions.LogFile,
				Common.Utilities.TerminalOptions.LogMode,
				Common.Utilities.TerminalOptions.OutputFile,
				TerminalOptions.StringCount,
				TerminalOptions.StringLength );

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
