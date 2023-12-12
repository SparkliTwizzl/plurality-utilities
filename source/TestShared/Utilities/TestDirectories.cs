using Petrichor.Common.Utilities;


namespace Petrichor.TestShared.Utilities
{
	public class TestDirectories : ProjectDirectories
	{
		private const string testDirectoryName = "_test";

		public static string TestDirectory => $@"{ BaseDirectory }\..\..\..\..\{ testDirectoryName }";
		public static string TestInputDirectory => $@"{ TestDirectory }\{ inputDirectoryName }";
		public static string TestLogDirectory => $@"{ TestDirectory }\{ logDirectoryName }";
		public static string TestOutputDirectory => $@"{ TestDirectory }\{ outputDirectoryName }";
	}
}
