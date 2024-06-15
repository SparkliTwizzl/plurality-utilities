using Petrichor.Common.Info;


namespace Petrichor.TestShared.Info
{
	public class TestDirectories : ProjectDirectories
	{
		private const string TestDirectoryName = "../../test";


		public static string TestDirectory => Path.Combine( DebugBaseDirectory, TestDirectoryName );
		public static string TestInputDirectory => Path.Combine( TestDirectory, InputDirectoryName );
		public static string TestLogDirectory => Path.Combine( TestDirectory, LogDirectoryName );
		public static string TestOutputDirectory => Path.Combine( TestDirectory, OutputDirectoryName );
	}
}
