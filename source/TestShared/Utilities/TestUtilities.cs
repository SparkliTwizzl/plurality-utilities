using Petrichor.Logging;
using Petrichor.TestShared.Info;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Log.EnableBuffering();
			Log.EnableTestMode();
			var label = GetCallingClassName();
			var logFileName = $"{label}_{DateTime.Now.ToString( "yyyy-MM-dd_hh-mm-ss.fffffff" )}.log";
			var logFilePath = Path.Combine( TestDirectories.TestLogDirectory, logFileName );
			Log.CreateLogFile( logFilePath );
			Log.WriteBufferToFile();
			Log.DisableBuffering();
		}

		public static string LocateInputFile( string fileName ) => $@"{TestDirectories.TestInputDirectory}\{fileName}";


		private static string GetCallingClassName()
		{
			var stackTrace = new System.Diagnostics.StackTrace();
			var callingMethod = stackTrace.GetFrame( 2 )?.GetMethod();
			var callingClassName = callingMethod?.DeclaringType?.Name ?? string.Empty;
			return $"{callingClassName}";
		}
	}
}
