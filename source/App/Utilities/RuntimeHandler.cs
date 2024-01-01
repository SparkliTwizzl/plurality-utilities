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
				var metadataRegionParser = new MetadataRegionParser();
				var moduleOptionsRegionParser = new ModuleOptionsRegionParser();
				var entryParser = new EntryParser();
				var entriesRegionParser = new EntriesRegionParser( entryParser );
				var templatesRegionParser = new TemplatesRegionParser();
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
				throw new ShortcutScriptGenerationException( "Generating AutoHotkey shortcuts script failed", exception );
			}
		}
	}
}
