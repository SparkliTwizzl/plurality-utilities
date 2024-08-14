namespace Petrichor.Common.Info
{
	public class ProjectDirectories
	{
		private const string BuildDirectoryName = "build";
		private const string InputDirectoryName = "input";
		private const string IntegrationTestDirectoryName = "integration";
		private const string LogDirectoryName = "log";
		private const string OutputDirectoryName = "output";
		private const string TestDirectoryName = "test";

		public static string BaseDirectory =>
#if DEBUG
			$"{AppContext.BaseDirectory}../../..";
#else
			AppContext.BaseDirectory;
#endif
		public static string BuildDirectory => Path.Combine( BaseDirectory, BuildDirectoryName );
		public static string InputDirectory =>
#if DEBUG
	Path.Combine( TestDirectory, InputDirectoryName );
#else
	Path.Combine( BaseDirectory, InputDirectoryName );
#endif
		public static string LogDirectory =>
#if DEBUG
		Path.Combine( TestLogDirectory, IntegrationTestDirectoryName );
#else
		Path.Combine( BaseDirectory, LogDirectoryName );
#endif

		public static string OutputDirectory =>
#if DEBUG
			Path.Combine( BaseDirectory, Path.Combine( TestDirectoryName, OutputDirectoryName ) );
#else
			Path.Combine( BaseDirectory, OutputDirectoryName );
#endif
		public static string TestDirectory => Path.Combine( BaseDirectory, TestDirectoryName );
		public static string TestInputDirectory => Path.Combine( TestDirectory, InputDirectoryName );
		public static string TestLogDirectory => Path.Combine( TestDirectory, LogDirectoryName );
		public static string TestOutputDirectory => OutputDirectory;


		private ProjectDirectories() { }
	}
}
