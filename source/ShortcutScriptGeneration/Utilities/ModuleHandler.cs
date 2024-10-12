using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Syntax;
using System.CommandLine;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Provides methods to handle text shortcut script generation commands.
	/// </summary>
	public static class ModuleHandler
	{
		/// <summary>
		/// Creates a terminal command for generating a text shortcut script.
		/// </summary>
		/// <returns>A <see cref="Command"/> configured for text shortcut script generation.</returns>
		public static Command CreateTerminalCommand()
		{
			var moduleCommand = new Command(
				name: Commands.ModuleCommand,
				description: "Parse input file and generate a text find-and-replace shortcut script." )
				{
					TerminalOptions.InputFile,
					TerminalOptions.OutputFile,
				};

			moduleCommand.SetHandler( ( autoExit, inputFile, logFile, logMode, outputFile ) =>
				{
					TerminalOptions.IsAutoExitEnabled = autoExit;
					MetadataHandler.CommandToRun = new()
					{
						Name = Commands.ModuleCommand,
						Options = new()
						{
							{ Common.Syntax.Commands.Parameters.AutoExit, autoExit.ToString() },
							{ Common.Syntax.Commands.Parameters.InputFile, inputFile },
							{ Common.Syntax.Commands.Parameters.LogFile, logFile },
							{ Common.Syntax.Commands.Parameters.LogMode, logMode },
							{ Common.Syntax.Commands.Parameters.OutputFile, outputFile },
						},
					};

					MetadataHandler.InitializeLogging( logMode, logFile );
					Logger.WriteBufferToFile();
					Logger.DisableBuffering();

					var inputFileHandler = new InputFileHandler( MetadataHandler.CreateMetadataTokenParser() );
					MetadataHandler.CommandToRun.Data = inputFileHandler.ProcessFile( inputFile ).ToArray();
				},
				TerminalOptions.AutoExit,
				TerminalOptions.InputFile,
				TerminalOptions.LogFile,
				TerminalOptions.LogMode,
				TerminalOptions.OutputFile );

			return moduleCommand;
		}

		/// <summary>
		/// Executes the text shortcut script generation command.
		/// </summary>
		/// <param name="command">The command containing the parameters for text shortcut script generation.</param>
		public static void ExecuteCommand( ModuleCommand command )
		{
			Logger.Important( "Generating text shortcuts script..." );
			var outputFilePath = command.Options[ Common.Syntax.Commands.Parameters.OutputFile ];
			GenerateAutoHotkeyScript( command.Data, outputFilePath );
			Logger.Important( "Generated text shortcuts script successfully." );
		}


		private static TokenBodyParser<Entry> CreateEntryParser()
		{
			var parserDescriptor = new TokenParseDescriptor<Entry>()
			{
				TokenPrototype = TokenPrototypes.Entry,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Color, EntryHandler.ColorTokenHandler },
					{ TokenPrototypes.Decoration, EntryHandler.DecorationTokenHandler },
					{ TokenPrototypes.ID, EntryHandler.IDTokenHandler },
					{ TokenPrototypes.Name, EntryHandler.NameTokenHandler },
					{ TokenPrototypes.LastName, EntryHandler.LastNameTokenHandler },
					{ TokenPrototypes.Pronoun, EntryHandler.PronounTokenHandler },
				},
			};

			return new TokenBodyParser<Entry>( parserDescriptor );
		}

		private static TokenBodyParser<List<Entry>> CreateEntryListParser()
		{
			var entryRegionParser = CreateEntryParser();

			var entryTokenHandler = ( IndexedString[] bodyData, int tokenStartIndex, List<Entry> result ) =>
			{
				entryRegionParser.Reset();
				var dataTrimmedToToken = bodyData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToToken );
				result.Add( entry );
				return new ProcessedTokenData<List<Entry>>( value: result, bodySize: entryRegionParser.TotalLinesParsed - 1 );
			};

			var parserDescriptor = new TokenParseDescriptor<List<Entry>>()
			{
				TokenPrototype = TokenPrototypes.EntryList,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Entry, entryTokenHandler },
				},
				PostParseAction = ( List<Entry> entries ) =>
				{
					Logger.Info( $"Parsed {entries.Count} \"{TokenPrototypes.Entry.Key}\" tokens." );
					return entries;
				},
			};

			return new TokenBodyParser<List<Entry>>( parserDescriptor );
		}

		private static TokenBodyParser<ModuleOptions> CreateModuleOptionsParser()
		{
			var parserDescriptor = new TokenParseDescriptor<ModuleOptions>()
			{
				TokenPrototype = TokenPrototypes.ModuleOptions,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.DefaultIcon, ModuleOptionsHandler.DefaultIconTokenHandler },
					{ TokenPrototypes.ReloadShortcut, ModuleOptionsHandler.ReloadShortcutTokenHandler },
					{ TokenPrototypes.SuspendIcon, ModuleOptionsHandler.SuspendIconTokenHandler },
					{ TokenPrototypes.SuspendShortcut, ModuleOptionsHandler.SuspendShortcutTokenHandler },
				},
			};

			return new TokenBodyParser<ModuleOptions>( parserDescriptor );
		}

		private static TokenBodyParser<TextShortcut> CreateShortcutTemplateParser()
		{
			var parserDescriptor = new TokenParseDescriptor<TextShortcut>()
			{
				TokenPrototype = TokenPrototypes.ShortcutTemplate,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Find, ShortcutHandler.FindTokenHandler },
					{ TokenPrototypes.Replace, ShortcutHandler.ReplaceTokenHandler },
					{ TokenPrototypes.TextCase, ShortcutHandler.TextCaseTokenHandler },
				},
				PostParseAction = ( TextShortcut template ) =>
				{
					if ( template.TextCase != TemplateTextCases.Default )
					{
						Logger.Info( $"Text case: {template.TextCase}." );
					}
					if ( template.FindAndReplace.Count > 0 )
					{
						Logger.Info( $"Parsed {template.FindAndReplace.Count} find-and-replace pairs." );
					}
					return template;
				},
			};

			var parser = new TokenBodyParser<TextShortcut>( parserDescriptor );

			var templateTokenHandler = ( IndexedString[] bodyData, int tokenStartIndex, TextShortcut result ) =>
			{
				var handlerResult = ShortcutHandler.ShortcutTemplateTokenHandler( bodyData, tokenStartIndex, result );
				result = handlerResult.Value;
				var nextToken = new StringToken( bodyData[ tokenStartIndex + 1 ] );
				if ( nextToken.TokenKey != Common.Syntax.TokenPrototypes.TokenBodyOpen.Key )
				{
					parser.CancelParsing();
				}
				return new ProcessedTokenData<TextShortcut>( result );
			};

			parser.AddTokenHandler( TokenPrototypes.ShortcutTemplate, templateTokenHandler );
			return parser;
		}

		private static TokenBodyParser<ShortcutScriptInput> CreateShortcutListParser()
		{
			var shortcutTemplateRegionParser = CreateShortcutTemplateParser();

			var shortcutTemplateTokenHandler = ( IndexedString[] bodyData, int tokenStartIndex, ShortcutScriptInput result ) =>
			{
				shortcutTemplateRegionParser.Reset();
				var dataTrimmedToToken = bodyData[ tokenStartIndex.. ];
				var template = shortcutTemplateRegionParser.Parse( dataTrimmedToToken );
				var templates = result.ShortcutTemplates.ToList();
				templates.Add( template );
				result.ShortcutTemplates = templates.ToArray();
				return new ProcessedTokenData<ShortcutScriptInput>( value: result, bodySize: shortcutTemplateRegionParser.TotalLinesParsed - 1 );
			};

			var parserDescriptor = new TokenParseDescriptor<ShortcutScriptInput>()
			{
				TokenPrototype = TokenPrototypes.ShortcutList,
				SubTokenHandlers = new()
				{
					{ TokenPrototypes.Shortcut, ShortcutHandler.ShortcutTokenHandler },
					{ TokenPrototypes.ShortcutTemplate, shortcutTemplateTokenHandler },
				},
				PreParseAction = ( ShortcutScriptInput input ) => input,
				PostParseAction = ( ShortcutScriptInput result ) =>
				{
					Logger.Info( $"Parsed {result.Shortcuts.Length} \"{TokenPrototypes.Shortcut.Key}\" tokens." );
					Logger.Info( $"Parsed {result.ShortcutTemplates.Length} \"{TokenPrototypes.ShortcutTemplate.Key}\" tokens." );
					return result;
				},
			};

			return new TokenBodyParser<ShortcutScriptInput>( parserDescriptor );
		}

		private static void GenerateAutoHotkeyScript( IndexedString[] data, string outputFilePath )
		{
			try
			{
				var inputHandler = new InputHandler(
					moduleOptionsParser: CreateModuleOptionsParser(),
					entryListParser: CreateEntryListParser(),
					shortcutListParser: CreateShortcutListParser(),
					shortcutProcessor: new ShortcutProcessor() );
				var input = inputHandler.ParseInputData( data );
				new ShortcutScriptGenerator().GenerateShortcutScriptFile( input, outputFilePath );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: {exception.Message}", exception ) );
			}
		}
	}
}
