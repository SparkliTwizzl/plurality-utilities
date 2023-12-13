namespace Petrichor.Common.Utilities
{
	public static class AppInfo
	{
		public const string AppName = "Petrichor";
		public const string AppNameAndVersion = $"{ AppName } v{ AppVersion }";
#if DEBUG
		public const string AppVersion = "0.8-dev";
#else
		public const string AppVersion = "0.8";
#endif
	}
}
