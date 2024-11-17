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
		private const string UnitTestDirectoryName = "unit";


		/// <summary>
		/// Gets the base directory of the project. Value changes depending on the build mode.
		/// </summary>
		public static string BaseDirectory =>
#if DEBUG
			$@"{AppContext.BaseDirectory}..\..";
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
			IntegrationTestInputDirectory;
#else
			Path.Combine(BaseDirectory, InputDirectoryName);
#endif

		/// <summary>
		/// Gets the log directory path. Value changes depending on the build mode.
		/// </summary>
		public static string LogDirectory =>
#if DEBUG
			IntegrationTestLogDirectory;
#else
			Path.Combine(BaseDirectory, LogDirectoryName);
#endif

		/// <summary>
		/// Gets the output directory path. Value changes depending on the build mode.
		/// </summary>
		public static string OutputDirectory =>
#if DEBUG
			IntegrationTestOutputDirectory;
#else
			Path.Combine(BaseDirectory, OutputDirectoryName);
#endif

		public static string TestDirectory => Path.Combine(BaseDirectory, TestDirectoryName);

		/// <summary>
		/// Gets the integration test directory path.
		/// </summary>
		public static string IntegrationTestDirectory => Path.Combine(BaseDirectory, TestDirectoryName);

		/// <summary>
		/// Gets the integration test input directory path.
		/// </summary>
		public static string IntegrationTestInputDirectory => Path.Combine(IntegrationTestDirectory, InputDirectoryName, IntegrationTestDirectoryName);

		/// <summary>
		/// Gets the integration test log directory path.
		/// </summary>
		public static string IntegrationTestLogDirectory => Path.Combine(IntegrationTestDirectory, LogDirectoryName, IntegrationTestDirectoryName);

		/// <summary>
		/// Gets the integration test output directory path.
		/// </summary>
		public static string IntegrationTestOutputDirectory => Path.Combine(IntegrationTestDirectory, OutputDirectoryName, IntegrationTestDirectoryName);

		/// <summary>
		/// Gets the unit test directory path.
		/// </summary>
		public static string UnitTestDirectory => Path.Combine(BaseDirectory, @"..\", TestDirectoryName);

		/// <summary>
		/// Gets the unit test input directory path.
		/// </summary>
		public static string UnitTestInputDirectory => Path.Combine(UnitTestDirectory, InputDirectoryName, UnitTestDirectoryName);

		/// <summary>
		/// Gets the unit test output directory path.
		/// </summary>
		public static string UnitTestOutputDirectory => Path.Combine(UnitTestDirectory, OutputDirectoryName, UnitTestDirectoryName);

		/// <summary>
		/// Gets the unit test log directory path.
		/// </summary>
		public static string UnitTestLogDirectory => Path.Combine(UnitTestDirectory, LogDirectoryName, UnitTestDirectoryName);


		/// <summary>
		/// Private constructor to prevent instantiation of this static class.
		/// </summary>
		private ProjectDirectories() { }
	}
}
