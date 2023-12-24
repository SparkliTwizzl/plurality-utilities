namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		private const string releaseAppVersion = "0.8";
		private const string developmentAppVersion = $"{releaseAppVersion}-dev";


		public const string AppName = "Petrichor";
		public static string AppNameAndVersion => $"{AppName} v{AppVersion}";
#if DEBUG
		public static string AppVersion => developmentAppVersion;
#else
		public static string AppVersion => releaseAppVersion;
#endif
	}
}
