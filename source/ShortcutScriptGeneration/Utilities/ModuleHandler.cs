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
	public static class ModuleHandler
	{
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
							{ Common.Syntax.Commands.Options.AutoExit, autoExit.ToString() },
							{ Common.Syntax.Commands.Options.InputFile, inputFile },
							{ Common.Syntax.Commands.Options.LogFile, logFile },
							{ Common.Syntax.Commands.Options.LogMode, logMode },
							{ Common.Syntax.Commands.Options.OutputFile, outputFile },
						},
					};

					MetadataHandler.InitializeLogging( logMode, logFile );
					Log.WriteBufferToFile();
					Log.DisableBuffering();

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

		public static void ExecuteCommand( ModuleCommand command )
		{
			Log.Important( "Generating text shortcuts script..." );
			var outputFilePath = command.Options[ Common.Syntax.Commands.Options.OutputFile ];
			GenerateAutoHotkeyScript( command.Data, outputFilePath );
			Log.Important( "Generated text shortcuts script successfully." );
		}


		private static TokenBodyParser<Entry> CreateEntryParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<Entry>()
			{
				RegionToken = Tokens.Entry,
				TokenHandlers = new()
				{
					{ Tokens.Color, EntryHandler.ColorTokenHandler },
					{ Tokens.Decoration, EntryHandler.DecorationTokenHandler },
					{ Tokens.ID, EntryHandler.IDTokenHandler },
					{ Tokens.Name, EntryHandler.NameTokenHandler },
					{ Tokens.LastName, EntryHandler.LastNameTokenHandler },
					{ Tokens.Pronoun, EntryHandler.PronounTokenHandler },
				},
			};

			return new TokenBodyParser<Entry>( parserDescriptor );
		}

		private static TokenBodyParser<List<Entry>> CreateEntryListParser()
		{
			var entryRegionParser = CreateEntryParser();

			var entryTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<Entry> result ) =>
			{
				entryRegionParser.Reset();
				var dataTrimmedToToken = regionData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToToken );
				result.Add( entry );
				return new ProcessedRegionData<List<Entry>>( value: result, bodySize: entryRegionParser.LinesParsed - 1 );
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<Entry>>()
			{
				RegionToken = Tokens.EntryList,
				TokenHandlers = new()
				{
					{ Tokens.Entry, entryTokenHandler },
				},
				PostParseHandler = ( List<Entry> entries ) =>
				{
					Log.Info( $"Parsed {entries.Count} \"{Tokens.Entry.Key}\" tokens." );
					return entries;
				},
			};

			return new TokenBodyParser<List<Entry>>( parserDescriptor );
		}

		private static TokenBodyParser<ModuleOptionData> CreateModuleOptionsParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ModuleOptionData>()
			{
				RegionToken = Tokens.ModuleOptions,
				TokenHandlers = new()
				{
					{ Tokens.DefaultIcon, ModuleOptionsHandler.DefaultIconTokenHandler },
					{ Tokens.ReloadShortcut, ModuleOptionsHandler.ReloadShortcutTokenHandler },
					{ Tokens.SuspendIcon, ModuleOptionsHandler.SuspendIconTokenHandler },
					{ Tokens.SuspendShortcut, ModuleOptionsHandler.SuspendShortcutTokenHandler },
				},
			};

			return new TokenBodyParser<ModuleOptionData>( parserDescriptor );
		}

		private static TokenBodyParser<ShortcutData> CreateShortcutTemplateParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ShortcutData>()
			{
				RegionToken = Tokens.ShortcutTemplate,
				TokenHandlers = new()
				{
					{ Tokens.Find, ShortcutHandler.FindTokenHandler },
					{ Tokens.Replace, ShortcutHandler.ReplaceTokenHandler },
					{ Tokens.TextCase, ShortcutHandler.TextCaseTokenHandler },
				},
				PostParseHandler = ( ShortcutData template ) =>
				{
					if ( template.TextCase != TemplateTextCases.Default )
					{
						Log.Info( $"Text case: {template.TextCase}." );
					}
					if ( template.FindAndReplace.Count > 0 )
					{
						Log.Info( $"Parsed {template.FindAndReplace.Count} find-and-replace pairs." );
					}
					return template;
				},
			};

			var parser = new TokenBodyParser<ShortcutData>( parserDescriptor );

			var templateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ShortcutData result ) =>
			{
				var handlerResult = ShortcutHandler.ShortcutTemplateTokenHandler( regionData, tokenStartIndex, result );
				result = handlerResult.Value;
				var nextToken = new StringToken( regionData[ tokenStartIndex + 1 ] );
				if ( nextToken.Key != Common.Syntax.Tokens.RegionOpen.Key )
				{
					parser.CancelParsing();
				}
				return new ProcessedRegionData<ShortcutData>( result );
			};

			parser.AddTokenHandler( Tokens.ShortcutTemplate, templateTokenHandler );
			return parser;
		}

		private static TokenBodyParser<InputData> CreateShortcutListParser()
		{
			var shortcutTemplateRegionParser = CreateShortcutTemplateParser();

			var shortcutTemplateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, InputData result ) =>
			{
				shortcutTemplateRegionParser.Reset();
				var dataTrimmedToToken = regionData[ tokenStartIndex.. ];
				var template = shortcutTemplateRegionParser.Parse( dataTrimmedToToken );
				var templates = result.ShortcutTemplates.ToList();
				templates.Add( template );
				result.ShortcutTemplates = templates.ToArray();
				return new ProcessedRegionData<InputData>( value: result, bodySize: shortcutTemplateRegionParser.LinesParsed - 1 );
			};

			var parserDescriptor = new DataRegionParserDescriptor<InputData>()
			{
				RegionToken = Tokens.ShortcutList,
				TokenHandlers = new()
				{
					{ Tokens.Shortcut, ShortcutHandler.ShortcutTokenHandler },
					{ Tokens.ShortcutTemplate, shortcutTemplateTokenHandler },
				},
				PreParseHandler = ( InputData input ) => input,
				PostParseHandler = ( InputData result ) =>
				{
					Log.Info( $"Parsed {result.Shortcuts.Length} \"{Tokens.Shortcut.Key}\" tokens." );
					Log.Info( $"Parsed {result.ShortcutTemplates.Length} \"{Tokens.ShortcutTemplate.Key}\" tokens." );
					return result;
				},
			};

			return new TokenBodyParser<InputData>( parserDescriptor );
		}

		private static void GenerateAutoHotkeyScript( IndexedString[] data, string outputFilePath )
		{
			try
			{
				var inputHandler = new InputHandler(
					moduleOptionsParser: CreateModuleOptionsParser(),
					entryListParser: CreateEntryListParser(),
					shortcutListParser: CreateShortcutListParser(),
					shortcutGenerator: new ShortcutProcessor() );
				var input = inputHandler.ParseFileData( data );
				new ScriptGenerator().Generate( input, outputFilePath );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: {exception.Message}", exception ) );
			}
		}
	}
}
