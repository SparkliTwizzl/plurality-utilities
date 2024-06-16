namespace Petrichor.Common.Info
{
	public class ProjectDirectories
	{
		protected static string DebugBaseDirectory => $@"{AppContext.BaseDirectory}../../..";
		protected static string ReleaseBaseDirectory => $"{AppContext.BaseDirectory}";
		protected const string BuildDirectoryName = "build";
		protected const string InputDirectoryName = "input";
		protected const string LogDirectoryName = "log";
		protected const string OutputDirectoryName = "output";


		public static string BaseDirectory =>
#if DEBUG
			DebugBaseDirectory;
#else
			ReleaseBaseDirectory;
#endif


		public static string BuildDirectory => $@"{BaseDirectory}\{BuildDirectoryName}";
		public static string InputDirectory => $@"{BaseDirectory}\{InputDirectoryName}";
		public static string LogDirectory => $@"{BaseDirectory}\{LogDirectoryName}";
		public static string OutputDirectory => $@"{BaseDirectory}\{OutputDirectoryName}";


		protected ProjectDirectories() { }
	}
}
