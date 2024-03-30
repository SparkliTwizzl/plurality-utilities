using Petrichor.App.Syntax;
using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public static class ShortcutScriptGenerationHandler
	{
		public static void GenerateScript( ModuleCommand command )
		{
			Log.Important( "Generating text shortcuts script..." );

			var outputFilePath = command.Options[ CommandOptions.ShortcutScriptOptionOutputFile ];
			var data = command.Data;
			GenerateAutoHotkeyScript( data, outputFilePath );

			var successMessage = "Generated text shortcuts script successfully.";
			if ( Log.IsLoggingToConsoleDisabled )
			{
				Console.WriteLine( successMessage );
			}
			Log.Important( successMessage );
		}


		private static DataRegionParser<ScriptEntry> CreateEntryRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ScriptEntry>()
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

			return new DataRegionParser<ScriptEntry>( parserDescriptor );
		}

		private static DataRegionParser<List<ScriptEntry>> CreateEntryListRegionParser()
		{
			var entryRegionParser = CreateEntryRegionParser();

			var entryTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<ScriptEntry> result ) =>
			{
				entryRegionParser.Reset();
				var dataTrimmedToRegion = regionData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToRegion );
				result.Add( entry );
				return new ProcessedRegionData<List<ScriptEntry>>( value: result, bodySize: entryRegionParser.LinesParsed - 1 );
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<ScriptEntry>>()
			{
				RegionToken = Tokens.EntryList,
				TokenHandlers = new()
				{
					{ Tokens.Entry, entryTokenHandler },
				},
				PostParseHandler = ( List<ScriptEntry> entries ) =>
				{
					Log.Info( $"Parsed {entries.Count} \"{Tokens.Entry.Key}\" tokens." );
					return entries;
				},
			};

			return new DataRegionParser<List<ScriptEntry>>( parserDescriptor );
		}

		private static DataRegionParser<ScriptModuleOptions> CreateModuleOptionsRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ScriptModuleOptions>()
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

			return new DataRegionParser<ScriptModuleOptions>( parserDescriptor );
		}

		private static DataRegionParser<ScriptMacroTemplate> CreateTemplateRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ScriptMacroTemplate>()
			{
				RegionToken = Tokens.Template,
				TokenHandlers = new()
				{
					{ Tokens.Find, TemplateHandler.FindTokenHandler },
					{ Tokens.Replace, TemplateHandler.ReplaceTokenHandler },
					{ Tokens.TextCase, TemplateHandler.TextCaseTokenHandler },
				},
				PostParseHandler = ( ScriptMacroTemplate template ) =>
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

			var parser = new DataRegionParser<ScriptMacroTemplate>( parserDescriptor );

			var templateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptMacroTemplate result ) =>
			{
				var handlerResult = TemplateHandler.TemplateTokenHandler( regionData, tokenStartIndex, result );
				result = handlerResult.Value;
				var nextToken = new StringToken( regionData[ tokenStartIndex + 1 ] );
				if ( nextToken.Key != Common.Syntax.Tokens.RegionOpen.Key )
				{
					parser.CancelParsing();
				}
				return new ProcessedRegionData<ScriptMacroTemplate>( result );
			};

			parser.AddTokenHandler( Tokens.Template, templateTokenHandler );
			return parser;
		}

		private static DataRegionParser<List<ScriptMacroTemplate>> CreateTemplateListRegionParser()
		{
			var templateRegionParser = CreateTemplateRegionParser();

			var templateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<ScriptMacroTemplate> result ) =>
			{
				templateRegionParser.Reset();
				var dataTrimmedToRegion = regionData[ tokenStartIndex.. ];
				var template = templateRegionParser.Parse( dataTrimmedToRegion );
				result.Add( template );
				return new ProcessedRegionData<List<ScriptMacroTemplate>>( value: result, bodySize: templateRegionParser.LinesParsed - 1 );
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<ScriptMacroTemplate>>()
			{
				RegionToken = Tokens.TemplateList,
				TokenHandlers = new()
				{
					{ Tokens.Template, templateTokenHandler },
				},
				PostParseHandler = ( List<ScriptMacroTemplate> templates ) =>
				{
					Log.Info( $"Parsed {templates.Count} \"{Tokens.Template.Key}\" tokens." );
					return templates;
				},
			};

			return new DataRegionParser<List<ScriptMacroTemplate>>( parserDescriptor );
		}

		private static void GenerateAutoHotkeyScript( IndexedString[] data, string outputFilePath )
		{
			try
			{
				var inputHandler = new ShortcutScriptGeneration.Utilities.InputHandler(
					moduleOptionsRegionParser: CreateModuleOptionsRegionParser(),
					entryListRegionParser: CreateEntryListRegionParser(),
					templateListRegionParser: CreateTemplateListRegionParser(),
					macroGenerator: new MacroGenerator() );
				var input = inputHandler.ParseRegionData( data );
				new ScriptGenerator().Generate( input, outputFilePath );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: {exception.Message}", exception ) );
			}
		}
	}
}
