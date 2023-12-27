namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		private const string releaseAppVersion = "0.8";
		private const string developmentAppVersion = $"{releaseAppVersion}-dev";


		public const string AppName = "Petrichor";
		public static string AppNameAndVersion => $"{AppName} v{AppVersion}";
		public static string AppVersion =>
#if DEBUG
			developmentAppVersion;
#else
			releaseAppVersion;
#endif
	}
}
