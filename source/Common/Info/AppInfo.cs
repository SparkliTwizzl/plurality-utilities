namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		public static string AppName => "Petrichor";
		public static string AppNameAndVersion => $"{AppName} {AppVersion}";
		public static string AppVersion =>
#if DEBUG
			DevelopmentAppVersion;
#else
			ReleaseAppVersion;
#endif
		public static string DevelopmentAppVersion => $"{ReleaseAppVersion}-dev";
		public static string MajorVersion => $"{MajorVersionValue}";
		public static int MajorVersionValue => 0;
		public static string MinorVersion => $"{MinorVersionValue}";
		public static int MinorVersionValue => 9;
		public static string PatchVersion => $"{PatchVersionValue}";
		public static int PatchVersionValue => 0;
		public static string PreReleaseVersion => "";
		public static int PreReleaseVersionValue => -1;
		public static string ReleaseAppVersion => $"{MajorVersion}.{MinorVersion}.{PatchVersion}{PreReleaseVersion}";
	}
}
