using System.Reflection;


namespace PluralityUtilities.Common.Utilities
{
	public static class ProjectDirectories
	{
		private const string buildDirectoryName = "_build";
		private const string inputDirectoryName = "_input";
		private const string logDirectoryName = "_log";
		private const string outputDirectoryName = "_output";
		private const string testDirectoryName = "_test";


		public static string BuildDirectory => $@"{ExecutingDirectory}\{buildDirectoryName}";
		public static string ExecutingDirectory => $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
		public static string LogDirectory => $@"{ExecutingDirectory}\{logDirectoryName}";
		public static string OutputDirectory => $@"{ExecutingDirectory}\{outputDirectoryName}";
		public static string TestDirectory => $@"{ExecutingDirectory}\{testDirectoryName}";
		public static string TestInputDirectory => $@"{TestDirectory}\{inputDirectoryName}";
		public static string TestLogDirectory => $@"{TestDirectory}\{logDirectoryName}";
		public static string TestOutputDirectory => $@"{TestDirectory}\{outputDirectoryName}";
	}
}
