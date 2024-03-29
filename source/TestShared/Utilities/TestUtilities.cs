using Petrichor.Logging;
using Petrichor.TestShared.Info;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Log.EnableAllLogDestinations();
			var logFileName = DateTime.Now.ToString( "test_yyyy-MM-dd_hh-mm-ss.log" );
			var logFilePath = Path.Combine( TestDirectories.TestLogDirectory, logFileName );
			Log.CreateLogFile( logFilePath );
		}

		public static string LocateInputFile( string fileName ) => $@"{TestDirectories.TestInputDirectory}\{fileName}";
	}
}
