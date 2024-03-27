using Petrichor.App.Syntax;
using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
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
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.Entry,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Color, EntryHandler.ColorTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Decoration, EntryHandler.DecorationTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.ID, EntryHandler.IDTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Name, EntryHandler.NameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.LastName, EntryHandler.LastNameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Pronoun, EntryHandler.PronounTokenHandler },
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
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.EntryList,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Entry, entryTokenHandler },
				},
				PostParseHandler = ( List<ScriptEntry> entries ) =>
				{
					Log.Info( $"Parsed {entries.Count} \"{ShortcutScriptGeneration.Syntax.Tokens.Entry.Key}\" tokens." );
					return entries;
				},
			};

			return new DataRegionParser<List<ScriptEntry>>( parserDescriptor );
		}

		private static DataRegionParser<ScriptModuleOptions> CreateModuleOptionsRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<ScriptModuleOptions>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.ModuleOptions,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.DefaultIcon, ModuleOptionsHandler.DefaultIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.ReloadShortcut, ModuleOptionsHandler.ReloadShortcutTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.SuspendIcon, ModuleOptionsHandler.SuspendIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.SuspendShortcut, ModuleOptionsHandler.SuspendShortcutTokenHandler },
				},
			};

			return new DataRegionParser<ScriptModuleOptions>( parserDescriptor );
		}

		private static DataRegionParser<List<string>> CreateTemplateListRegionParser()
		{
			var parserDescriptor = new DataRegionParserDescriptor<List<string>>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.TemplateList,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Template, TemplateHandler.TokenHandler },
				},
				PostParseHandler = ( List<string> templates ) =>
				{
					Log.Info( $"Parsed {templates.Count} \"{ShortcutScriptGeneration.Syntax.Tokens.Template.Key}\" tokens." );
					return templates;
				},
			};

			return new DataRegionParser<List<string>>( parserDescriptor );
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
