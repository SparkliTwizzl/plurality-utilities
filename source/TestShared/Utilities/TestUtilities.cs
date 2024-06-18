using Petrichor.Common.Info;
using Petrichor.Logging;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Log.EnableBuffering();
			Log.EnableTestMode();
			var label = GetCallingClassName();
			var logFileName = $"{label}_{DateTime.Now:yyyy-MM-dd_hh-mm-ss_fffffff}.log";
			var logFilePath = Path.Combine( ProjectDirectories.TestLogDirectory, logFileName );
			Log.CreateLogFile( logFilePath );
			Log.WriteBufferToFile();
			Log.DisableBuffering();
		}

		public static string LocateInputFile( string fileName ) => $@"{ProjectDirectories.TestInputDirectory}\{fileName}";


		private static string GetCallingClassName()
		{
			var stackTrace = new System.Diagnostics.StackTrace();
			var callingMethod = stackTrace.GetFrame( 2 )?.GetMethod();
			var callingClassName = callingMethod?.DeclaringType?.Name ?? string.Empty;
			return $"{callingClassName}";
		}
	}
}
