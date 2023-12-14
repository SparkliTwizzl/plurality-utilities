using Petrichor.Common.Info;


namespace Petrichor.TestShared.Info
{
	public class TestDirectories : ProjectDirectories
	{
		private const string testDirectoryName = "_test";


		public static string TestDirectory => $@"{debugBaseDirectory}\{testDirectoryName}";
		public static string TestInputDirectory => $@"{TestDirectory}\{inputDirectoryName}";
		public static string TestLogDirectory => $@"{TestDirectory}\{logDirectoryName}";
		public static string TestOutputDirectory => $@"{TestDirectory}\{outputDirectoryName}";
	}
}
