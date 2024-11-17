using Petrichor.TestShared.Utilities;


namespace Petrichor.TestShared.Info
{
	public struct TestAssets
	{
		public static string DefaultIconFileName => "IconDefault.ico";
		public static string DefaultIconFilePath => TestUtilities.LocateInputFile(DefaultIconFileName);
		public static string ReloadShortcut => "[win]r";
		public static string SuspendIconFileName => "IconSuspend.ico";
		public static string SuspendIconFilePath => TestUtilities.LocateInputFile(SuspendIconFileName);
		public static string SuspendShortcut => "#s";
	}
}
