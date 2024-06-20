namespace Petrichor.Common.Info
{
	public class ProjectDirectories
	{
		public static string BaseDirectory =>
#if DEBUG
			$"{AppContext.BaseDirectory}../../..";
#else
			AppContext.BaseDirectory;
#endif

		private const string OutputDirectoryName = "output";
		public static string OutputDirectory =>
#if DEBUG
			Path.Combine( BaseDirectory, Path.Combine( TestDirectoryName, OutputDirectoryName ) );
#else
			Path.Combine( BaseDirectory, OutputDirectoryName );
#endif

		private const string BuildDirectoryName = "build";
		private const string InputDirectoryName = "input";
		private const string LogDirectoryName = "log";
		private const string TestDirectoryName = "test";


		public static string BuildDirectory => Path.Combine( BaseDirectory, BuildDirectoryName );
		public static string InputDirectory => Path.Combine( BaseDirectory, InputDirectoryName );
		public static string LogDirectory => Path.Combine( BaseDirectory, LogDirectoryName );
		
		public static string TestDirectory => Path.Combine( BaseDirectory, TestDirectoryName );
		public static string TestInputDirectory => Path.Combine( TestDirectory, InputDirectoryName );
		public static string TestLogDirectory => Path.Combine( TestDirectory, LogDirectoryName );
		public static string TestOutputDirectory => OutputDirectory;


		private ProjectDirectories() { }
	}
}
