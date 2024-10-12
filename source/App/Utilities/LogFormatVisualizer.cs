using Petrichor.Logging;
using Petrichor.Logging.Styling;

namespace Petrichor.App.Utilities
{
	/// <summary>
	/// A utility class for visualizing different log message formats in debug mode.
	/// </summary>
	public static class LogFormatVisualizer
	{
		/// <summary>
		/// Displays test log messages with various log levels and custom formatting to the console in debug mode.
		/// This is used to verify the appearance of formatted log messages during development.
		/// </summary>
		public static void ShowTestMessagesInDebug()
		{
#if DEBUG
			Console.WriteLine("========== START FORMATTED LOG MESSAGE TEST ==========");
			Logger.Important("test");
			Logger.Info("test");
			Logger.Start("test");
			Logger.Finish("test");
			Logger.Warning("test");
			Logger.Error("test");
			Logger.Debug("test");
			var custom = new MessageFormat
			{
				ForegroundColor = "#fdb975",
				BackgroundColor = "#02468a",
				Label = "custom",
			};
			Logger.Formatted("test", custom);
			Console.WriteLine("=========== END FORMATTED LOG MESSAGE TEST ===========");
#endif
		}
	}
}
