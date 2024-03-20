using Petrichor.App.Enums;
using Petrichor.Logging.Enums;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static LogMode ActiveLogMode { get; set; } = LogMode.None;
		public static string InputFilePath { get; set; } = string.Empty;
		public static string OutputFilePath { get; set; } = string.Empty;


		public static void Execute( Module module )
		{
			switch ( module )
			{
				case Module.ShortcutScriptGeneration:
					ShortcutScriptGenerationHandler.GenerateScript( InputFilePath, OutputFilePath );
					break;

				case Module.Some:
					Console.WriteLine( "Unrecognized command." );
					break;

				case Module.None:
					Console.WriteLine( "No command provided." );
					break;

				default:
					break;
			}
		}

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}
	}
}
