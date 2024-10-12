using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;
using System.Data;


namespace Petrichor.Common.Info
{
	/// <summary>
	/// Provides versioning information and utilities for the application.
	/// </summary>
	public static class AppVersion
	{
		/// <summary>
		/// The current version of the application, dynamically selected based on build type.
		/// </summary>
		public static string Current =>
#if DEBUG
			CurrentDevelopment;
#else
			CurrentRelease;
#endif

		/// <summary>
		/// The current development version of the application.
		/// </summary>
		public static string CurrentDevelopment => $"{CurrentRelease}{DevelopmentVersionSuffix}";

		/// <summary>
		/// The suffix used for marking development versions.
		/// </summary>
		public const string DevelopmentVersionSuffix = "-dev";

		/// <summary>
		/// The major version number.
		/// </summary>
		public const string Major = "0";

		/// <summary>
		/// The minor version number.
		/// </summary>
		public const string Minor = "12";

		/// <summary>
		/// The patch version number.
		/// </summary>
		public const string Patch = "2";

		/// <summary>
		/// Optional preview version identifier, empty for stable releases.
		/// </summary>
		public const string Preview = "";

		/// <summary>
		/// The current release version of the application.
		/// </summary>
		public static string CurrentRelease => $"{Major}.{Minor}.{Patch}{Preview}";

		/// <summary>
		/// Version numbers supported by the application.
		/// </summary>
		public static string[] SupportedVersions =>
		[
			Current,
			$"{Major}.{Minor}.{Patch}",
			$"{Major}.{Minor}.0",
			"0.12.1",
			"0.12.0",
			"0.11.1",
			"0.11.0",
		];

		/// <summary>
		/// Checks if a given version is supported by the application.
		/// </summary>
		/// <param name="version">The version string to check.</param>
		/// <returns>True if the version is supported, otherwise false.</returns>
		public static bool IsVersionSupported(string version)
		{
#if DEBUG
			if (version.Contains(DevelopmentVersionSuffix))
			{
				var suffixStartIndex = version.IndexOf(DevelopmentVersionSuffix);
				version = version[..suffixStartIndex];
			}
#endif
			var versionComponents = version.Split('.');
			var major = versionComponents.Length > 0 ? versionComponents[0] : "invalid";
			var minor = versionComponents.Length > 1 ? versionComponents[1] : "invalid";
			var patch = versionComponents.Length > 2 ? versionComponents[2] : "0";
			var preview = versionComponents.Length > 3 ? versionComponents[3] : string.Empty;

			var formattedVersion = $"{major}.{minor}.{patch}{preview}";
			return SupportedVersions.Contains(formattedVersion);
		}

		/// <summary>
		/// Rejects versions that are not supported, logging and throwing an exception if necessary.
		/// </summary>
		/// <param name="version">The version to check.</param>
		/// <param name="lineNumber">Optional line number for logging context.</param>
		public static void RejectUnsupportedVersions(string version, int? lineNumber = null)
		{
			if (string.IsNullOrEmpty(version))
			{
				ExceptionLogger.LogAndThrow(new VersionNotFoundException("Version cannot be blank."), lineNumber);
			}

			if (!IsVersionSupported(version))
			{
				ExceptionLogger.LogAndThrow(new VersionNotFoundException($"Version \"{version}\" is not supported by {AppInfo.AppNameAndVersion}."), lineNumber);
			}

			Logger.Info($"Version \"{version}\" is compatible with {AppInfo.AppNameAndVersion}.");
		}
	}
}
