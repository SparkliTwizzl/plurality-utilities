using Petrichor.Logging;
using Petrichor.TestShared.Info;


namespace Petrichor.TestShared.Utilities
{
	public static class TestUtilities
	{
		public static void InitializeLoggingForTests()
		{
			Log.EnableForAll( TestDirectories.TestLogDirectory );
			Log.SetLogFileName( DateTime.Now.ToString( "test_yyyy-MM-dd_hh-mm-ss.log" ) );
		}

		public static string LocateInputFile( string fileName )
		{
			return $@"{ TestDirectories.TestInputDirectory }\{ fileName }";
		}
	}
}
