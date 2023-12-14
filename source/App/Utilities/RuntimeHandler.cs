using Petrichor.Common.Utilities;
using Petrichor.Logging.Enums;
using Petrichor.Logging;
using Petrichor.AutoHotkeyScripts.Utilities;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		private static string inputFilePath = string.Empty;
		private static LogMode activeLogMode = LogMode.Disabled;
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
				var entryParser = new EntryParser();
				var templateParser = new TemplateParser();
				var inputParser = new InputParser(entryParser, templateParser);
				var scriptGenerator = new AutoHotkeyScriptGenerator();
				var input = inputParser.ParseInputFile(InputFilePath);
				var macros = scriptGenerator.GenerateMacrosFromInput(input);
				scriptGenerator.GenerateScript(macros, OutputFilePath);

				var successMessage = "generated AutoHotkey shortcuts script successfully";
				if (ActiveLogMode != LogMode.All)
				{
					Console.WriteLine(successMessage);
				}
				Log.Important(successMessage);
			}
			catch (Exception ex)
			{
				var errorMessage = $"generating AutoHotkey shortcuts script failed with error: {ex.Message}";
				if (ActiveLogMode != LogMode.All)
				{
					Console.WriteLine(errorMessage);
				}
				Log.Error(errorMessage);
			}
		}
	}
}
