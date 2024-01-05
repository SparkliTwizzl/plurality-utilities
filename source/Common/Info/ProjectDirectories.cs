namespace Petrichor.Common.Info
{
	public class ProjectDirectories
	{
		protected static readonly string debugBaseDirectory = $@"{AppContext.BaseDirectory}..\..\..\..";
		protected static readonly string releaseBaseDirectory = $"{AppContext.BaseDirectory}";
		protected const string buildDirectoryName = "_build";
		protected const string inputDirectoryName = "_input";
		protected const string logDirectoryName = "_log";
		protected const string outputDirectoryName = "_output";


		public static string BaseDirectory =>
#if DEBUG
			debugBaseDirectory;
#else
			releaseBaseDirectory;
#endif


		public static string BuildDirectory => $@"{BaseDirectory}\{buildDirectoryName}";
		public static string InputDirectory => $@"{BaseDirectory}\{inputDirectoryName}";
		public static string LogDirectory => $@"{BaseDirectory}\{logDirectoryName}";
		public static string OutputDirectory => $@"{BaseDirectory}\{outputDirectoryName}";


		protected ProjectDirectories() { }
	}
}
