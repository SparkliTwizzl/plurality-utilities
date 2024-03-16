using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static string InputFilePath { get; set; } = string.Empty;
		public static LogMode ActiveLogMode { get; set; } = LogMode.None;
		public static string OutputFilePath { get; set; } = string.Empty;


		public static void Execute() => CreateAutoHotkeyScript();

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}


		private static void CreateAutoHotkeyScript()
		{
			try
			{
				Log.Important( "Generating AutoHotkey shortcuts script..." );
				
				var metadataRegionHandler = new MetadataRegionHandler();

				var moduleOptionsRegionParser = new ModuleOptionsRegionParser();
				var entryRegionParser = new EntryRegionParser();
				var entriesRegionParser = new EntriesRegionParser( entryRegionParser );
				var templatesRegionParser = new TemplatesRegionParser();

				var macroGenerator = new MacroGenerator();

				var inputFileParser = new InputFileHandler( metadataRegionHandler.Parser, moduleOptionsRegionParser, entriesRegionParser, templatesRegionParser, macroGenerator );
				var input = inputFileParser.ProcessFile( InputFilePath );
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
				ExceptionLogger.LogAndThrow( new ShortcutScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: {exception.Message}", exception ) );
			}
		}
	}
}
