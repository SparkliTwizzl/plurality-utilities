using Petrichor.Logging;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Log.SetLogFolder( TestDirectories.TestLogDirectory );
			Log.SetLogFileName( DateTime.Now.ToString( "test_yyyy-MM-dd_hh-mm-ss.log" ) );
			Log.EnableInVerboseMode();
		}

		public static string LocateInputFile( string fileName )
		{
			return $@"{ TestDirectories.TestInputDirectory }\{ fileName }";
		}
	}
}
