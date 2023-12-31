namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		public static string AppName => "Petrichor";
		public static string AppNameAndVersion => $"{AppName} {AppVersion.Current}";
	}
}
