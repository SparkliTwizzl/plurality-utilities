namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		public static string AppName => "Petrichor";
		public static string AppNameAndVersion => $"{AppName} v{AppVersion}";
		public static string AppVersion =>
#if DEBUG
			DevelopmentAppVersion;
#else
			ReleaseAppVersion;
#endif
		public static string DevelopmentAppVersion => $"{ReleaseAppVersion}-dev";
		public static string MajorVersion => "0";
		public static string MinorVersion => "9";
		public static string PatchVersion => "0";
		public static string PreReleaseVersion => "";
		public static string ReleaseAppVersion => $"{MajorVersion}.{MinorVersion}.{PatchVersion}{PreReleaseVersion}";
	}
}
