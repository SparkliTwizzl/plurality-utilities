namespace Petrichor.Common.Info
{
	/// <summary>
	/// Contains information about the application.
	/// </summary>
	public struct AppInfo
	{
		/// <summary>
		/// The name of the application.
		/// </summary>
		public const string AppName = "Petrichor";

		/// <summary>
		/// The application name and current version.
		/// </summary>
		public static string AppNameAndVersion => $"{AppName} {AppVersion.Current}";
	}
}
