namespace Petrichor.TestShared.Info
{
	public struct TestAssets
	{
		public static string DefaultIconFileName => "IconDefault.ico";
		public static string DefaultIconFilePath => $@"{ TestDirectories.TestInputDirectory }\{ DefaultIconFileName }";
		public static string ReloadShortcut => "#*";
		public static string SuspendIconFileName => "IconSuspend.ico";
		public static string SuspendIconFilePath => $@"{ TestDirectories.TestInputDirectory }\{ SuspendIconFileName }";
	}
}
