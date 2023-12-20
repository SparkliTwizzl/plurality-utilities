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

		public static string[] SplitRegionDataString(string regionDataString)
		{
			Log.Info($"raw data: \"{regionDataString}\"");
			var tokens = regionDataString.Split('|');
			Log.Info("tokenized data:");
			foreach (var token in tokens)
			{
				Log.Info($"	\"{token}\"");
			}
			Log.Info();
			return tokens;
		}
	}
}
