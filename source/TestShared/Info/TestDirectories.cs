using Petrichor.Common.Info;


namespace Petrichor.TestShared.Info
{
	public class TestDirectories : ProjectDirectories
	{
		private const string TestDirectoryName = "_test";


		public static string TestDirectory => $@"{DebugBaseDirectory}\{TestDirectoryName}";
		public static string TestInputDirectory => $@"{TestDirectory}\{InputDirectoryName}";
		public static string TestLogDirectory => $@"{TestDirectory}\{LogDirectoryName}";
		public static string TestOutputDirectory => $@"{TestDirectory}\{OutputDirectoryName}";
	}
}
