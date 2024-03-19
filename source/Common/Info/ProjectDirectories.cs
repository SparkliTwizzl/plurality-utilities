namespace Petrichor.Common.Info
{
	public class ProjectDirectories
	{
		protected static string DebugBaseDirectory => $@"{AppContext.BaseDirectory}..\..\..\..";
		protected static string ReleaseBaseDirectory => $"{AppContext.BaseDirectory}";
		protected const string BuildDirectoryName = "_build";
		protected const string InputDirectoryName = "_input";
		protected const string LogDirectoryName = "_log";
		protected const string OutputDirectoryName = "_output";


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
