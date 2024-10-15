namespace Petrichor.Common.Info
{
	/// <summary>
	/// Provides paths to important project directories.
	/// </summary>
	public class ProjectDirectories
	{
		private const string BuildDirectoryName = "build";
		private const string InputDirectoryName = "input";
		private const string IntegrationTestDirectoryName = "integration";
		private const string LogDirectoryName = "log";
		private const string OutputDirectoryName = "output";
		private const string TestDirectoryName = "test";


		/// <summary>
		/// Gets the base directory of the project. Value changes depending on the build mode.
		/// </summary>
		public static string BaseDirectory =>
#if DEBUG
			$"{AppContext.BaseDirectory}../..";
#else
			AppContext.BaseDirectory;
#endif

		/// <summary>
		/// Gets the build directory path.
		/// </summary>
		public static string BuildDirectory => Path.Combine(BaseDirectory, BuildDirectoryName);

		/// <summary>
		/// Gets the input directory path. Value changes depending on the build mode.
		/// </summary>
		public static string InputDirectory =>
#if DEBUG
			Path.Combine(TestDirectory, InputDirectoryName);
#else
			Path.Combine(BaseDirectory, InputDirectoryName);
#endif

		/// <summary>
		/// Gets the log directory path. Value changes depending on the build mode.
		/// </summary>
		public static string LogDirectory =>
#if DEBUG
			Path.Combine(TestLogDirectory, IntegrationTestDirectoryName);
#else
			Path.Combine(BaseDirectory, LogDirectoryName);
#endif

		/// <summary>
		/// Gets the output directory path. Value changes depending on the build mode.
		/// </summary>
		public static string OutputDirectory =>
#if DEBUG
			Path.Combine(BaseDirectory, Path.Combine(TestDirectoryName, OutputDirectoryName));
#else
			Path.Combine(BaseDirectory, OutputDirectoryName);
#endif

		/// <summary>
		/// Gets the test directory path.
		/// </summary>
		public static string TestDirectory => Path.Combine(BaseDirectory, TestDirectoryName);

		/// <summary>
		/// Gets the test input directory path.
		/// </summary>
		public static string TestInputDirectory => Path.Combine(TestDirectory, InputDirectoryName);

		/// <summary>
		/// Gets the test log directory path.
		/// </summary>
		public static string TestLogDirectory => Path.Combine(TestDirectory, LogDirectoryName);

		/// <summary>
		/// Gets the test output directory path.
		/// </summary>
		public static string TestOutputDirectory => OutputDirectory;


		// Private constructor to prevent instantiation of this static class.
		private ProjectDirectories() { }
	}
}
