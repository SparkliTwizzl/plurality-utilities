namespace Petrichor.TestShared.Info
{
	public struct TestAssets
	{
		private const string defaultIconFileName = "IconDefault.ico";
		private const string suspendIconFileName = "IconSuspend.ico";


		public static string DefaultIconFileName => defaultIconFileName;
		public static string DefaultIconFilePath => $@"{ TestDirectories.TestInputDirectory }\{ DefaultIconFileName }";
		public static string SuspendIconFileName => suspendIconFileName;
		public static string SuspendIconFilePath => $@"{ TestDirectories.TestInputDirectory }\{ SuspendIconFileName }";
	}
}
