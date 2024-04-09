using Petrichor.Logging;


namespace Petrichor.App.Utilities
{
	public static class LogFormatVisualizer
	{
		public static void ShowTestMessagesInDebug()
		{
#if DEBUG
			Console.WriteLine( "========== START FORMATTED LOG MESSAGE TEST ==========" );
			Log.Important( "test" );
			Log.Info( "test" );
			Log.Start( "test" );
			Log.Finish( "test" );
			Log.Warning( "test" );
			Log.Error( "test" );
			Log.Debug( "test" );

			var customColors = new Log.ColorScheme
			{
				Foreground = "#fdb975",
				Background = "#02468a",
			};
			Log.Formatted( "custom", "test", customColors );
			Console.WriteLine( "========== END FORMATTED LOG MESSAGE TEST ==========" );
#endif
		}
	}
}
