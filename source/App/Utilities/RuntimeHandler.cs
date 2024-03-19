using Petrichor.Logging.Enums;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static LogMode ActiveLogMode { get; set; } = LogMode.None;
		public static string InputFilePath { get; set; } = string.Empty;
		public static string OutputFilePath { get; set; } = string.Empty;


		public static void Execute() => ShortcutScriptGenerationHandler.GenerateScript( InputFilePath, OutputFilePath );

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}
	}
}
