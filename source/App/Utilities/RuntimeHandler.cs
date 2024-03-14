using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static string InputFilePath { get; set; } = string.Empty;
		public static LogMode ActiveLogMode { get; set; } = LogMode.None;
		public static string OutputFilePath { get; set; } = string.Empty;


		public static void Execute() => CreateTextShortcutScript();

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}


		private static void CreateTextShortcutScript()
		{
			try
			{
				Log.Important( "Generating AutoHotkey shortcuts script..." );

				var metadataRegionTokenHandlers = new Dictionary< string, Func< string[], int, StringWrapper, DataToken< StringWrapper >>>()
				{
				};
				var metadataRegionParser = new RegionParser< StringWrapper >(
					Common.Syntax.TokenNames.MetadataRegion,
					Common.Info.DataRegionInfo.MetadataRegionsAllowed,
					metadataRegionTokenHandlers );

				var moduleOptionsRegionTokenHandlers = new Dictionary< string, Func< string[], int, ScriptModuleOptions, DataToken< ScriptModuleOptions >>>()
				{
				};
				var moduleOptionsRegionParser = new RegionParser< ScriptModuleOptions >(
					ShortcutScriptGeneration.Syntax.TokenNames.ModuleOptionsRegion,
					ShortcutScriptGeneration.Info.DataRegionInfo.ModuleOptionsRegionsAllowed,
					moduleOptionsRegionTokenHandlers );

				var entryRegionTokenHandlers = new Dictionary< string, Func< string[], int, ScriptEntry, DataToken< ScriptEntry >>>()
				{
				};
				var entryRegionParser = new RegionParser< ScriptEntry >(
					ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion,
					ShortcutScriptGeneration.Info.DataRegionInfo.EntryRegionsAllowed,
					entryRegionTokenHandlers );

				var entriesRegionTokenHandlers = new Dictionary< string, Func< string[], int, List< ScriptEntry >, DataToken< List< ScriptEntry >>>>()
				{
				};
				var entriesRegionParser = new RegionParser< List< ScriptEntry >>(
					ShortcutScriptGeneration.Syntax.TokenNames.EntriesRegion,
					ShortcutScriptGeneration.Info.DataRegionInfo.EntriesRegionsAllowed,
					entriesRegionTokenHandlers );

				var templatesRegionTokenHandlers = new Dictionary< string, Func< string[], int, List< string >, DataToken< List< string >>>>()
				{
				};
				var templatesRegionParser = new RegionParser< List< string > >(
					ShortcutScriptGeneration.Syntax.TokenNames.TemplatesRegion,
					ShortcutScriptGeneration.Info.DataRegionInfo.TemplatesRegionsAllowed,
					templatesRegionTokenHandlers );

				var macroGenerator = new MacroGenerator();
				var inputFileParser = new InputFileParser( metadataRegionParser, moduleOptionsRegionParser, entriesRegionParser, templatesRegionParser, macroGenerator );

				var input = inputFileParser.Parse( InputFilePath );
				var scriptGenerator = new ScriptGenerator( input );
				scriptGenerator.Generate( OutputFilePath );

				var successMessage = "Generated AutoHotkey shortcuts script successfully";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( successMessage );
				}
				Log.Important( successMessage );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ShortcutScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: { exception.Message }", exception ) );
			}
		}
	}
}
