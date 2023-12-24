using Petrichor.Logging.Enums;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		private static string inputFilePath = string.Empty;
		private static LogMode activeLogMode = LogMode.None;
		private static string outputFilePath = string.Empty;

		public static string InputFilePath
		{
			get => inputFilePath;
			set => inputFilePath = value;
		}
		public static LogMode ActiveLogMode
		{
			get => activeLogMode;
			set => activeLogMode = value;
		}
		public static string OutputFilePath
		{
			get => outputFilePath;
			set => outputFilePath = value;
		}


		public static void Execute()
		{
			CreateAutoHotkeyScript();
		}

		public static void WaitForUserAndExit()
		{
			Console.Write("press any key to exit");
			Console.ReadKey(true);
			Environment.Exit(0);
		}


		private static void CreateAutoHotkeyScript()
		{
			try
			{
				Log.Important("generating AutoHotkey shortcuts script...");
				var metadataParser = new ShortcutScriptMetadataParser();
				var entryParser = new ShortcutScriptEntryParser();
				var templateParser = new ShortcutScriptTemplateParser();
				var macroParser = new ShortcutScriptMacroParser();
				var inputParser = new ShortcutScriptInputParser(metadataParser, entryParser, templateParser, macroParser);

				var input = inputParser.ParseInputFile(InputFilePath);
				var scriptGenerator = new ShortcutScriptGenerator( input );
				scriptGenerator.GenerateScript(OutputFilePath);

				var successMessage = "generated AutoHotkey shortcuts script successfully";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine(successMessage);
				}
				Log.Important(successMessage);
			}
			catch (Exception ex)
			{
				var errorMessage = $"generating AutoHotkey shortcuts script failed with error: { ex.Message }";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine(errorMessage);
				}
				Log.Error(errorMessage);
			}
		}
	}
}
