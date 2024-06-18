using Petrichor.Common.Info;


namespace Petrichor.TestShared.Info
{
	public struct TestAssets
	{
		public static string DefaultIconFileName => "IconDefault.ico";
		public static string DefaultIconFilePath => $@"{ProjectDirectories.TestInputDirectory}\{DefaultIconFileName}";
		public static string ReloadShortcut => "#r";
		public static string SuspendIconFileName => "IconSuspend.ico";
		public static string SuspendIconFilePath => $@"{ProjectDirectories.TestInputDirectory}\{SuspendIconFileName}";
		public static string SuspendShortcut => "#s";
	}
}
