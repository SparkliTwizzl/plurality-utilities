using Petrichor.Logging;
using Petrichor.Logging.Enums;
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
			Console.Write( "press any key to exit" );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}


		private static void CreateAutoHotkeyScript()
		{
			try
			{
				Log.Important( "generating AutoHotkey shortcuts script..." );
				var moduleOptionsParser = new ShortcutScriptModuleOptionsParser();
				var entryParser = new ShortcutScriptEntryParser();
				var templateParser = new ShortcutScriptTemplateParser();
				var macroParser = new ShortcutScriptMacroParser();
				var inputParser = new ShortcutScriptInputParser( moduleOptionsParser, entryParser, templateParser, macroParser );

				var input = inputParser.ParseInputFile( InputFilePath );
				var scriptGenerator = new ShortcutScriptGenerator( input );
				scriptGenerator.GenerateScript( OutputFilePath );

				var successMessage = "generated AutoHotkey shortcuts script successfully";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( successMessage );
				}
				Log.Important( successMessage );
			}
			catch ( Exception ex )
			{
				var errorMessage = $"generating AutoHotkey shortcuts script failed with error: {ex.Message}";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( errorMessage );
				}
				Log.Error( errorMessage );
			}
		}
	}
}
