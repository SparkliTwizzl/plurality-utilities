namespace Petrichor.Common.Info
{
	public struct AppInfo
	{
		public const string AppName = "Petrichor";
		public static string AppNameAndVersion => $"{AppName} {AppVersion.Current}";
	}
}
