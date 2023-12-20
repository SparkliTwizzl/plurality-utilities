namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		private const string releaseAppVerion = "0.8";
		private const string developmentAppVersion = $"{releaseAppVerion}-dev";


		public const string AppName = "Petrichor";
		public const string AppNameAndVersion = $"{AppName} v{AppVersion}";
#if DEBUG
		public const string AppVersion = developmentAppVersion;
#else
		public const string AppVersion = releaseAppVersion;
#endif
	}
}
