using Petrichor.Common.Info;
using Petrichor.Logging;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Logger.EnableBuffering();
			Logger.EnableTestMode();
			var label = GetCallingClassName();
			var logFileName = $"{label}_{DateTime.Now:yyyy-MM-dd_hh-mm-ss_fffffff}.log";
			var logFileDirectory = Path.Combine( ProjectDirectories.TestLogDirectory, "unit" );
			var logFilePath = Path.Combine( logFileDirectory, logFileName );
			Logger.CreateLogFile( logFilePath );
			Logger.WriteBufferToFile();
			Logger.DisableBuffering();
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
