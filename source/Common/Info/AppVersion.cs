using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.Data;


namespace Petrichor.Common.Info
{
	public static class AppVersion
	{
		public static string Current =>
#if DEBUG
			DevelopmentAppVersion;
#else
			ReleaseAppVersion;
#endif
		public static string DevelopmentAppVersion => $"{ReleaseAppVersion}{DevelopmentAppVersionSuffix}";
		public const string DevelopmentAppVersionSuffix = "-dev";
		public const string Major = "0";
		public const string Minor = "12";
		public const string Patch = "0";
		public const string Preview = "";
		public static string ReleaseAppVersion => $"{Major}.{Minor}.{Patch}{Preview}";
		public static string[] SupportedVersions => new[]
		{
			Current,
			$"{Major}.{Minor}.{Patch}.0",
			$"{Major}.{Minor}.0.0",
			"0.11.0",
			"0.11.1",
		};



		public static bool IsVersionSupported( string version )
		{
#if DEBUG
			if ( version.Contains( DevelopmentAppVersionSuffix ) )
			{
				var suffixStartsAt = version.IndexOf( DevelopmentAppVersionSuffix );
				version = version[ ..suffixStartsAt ];
			}
#endif
			var versionComponents = version.Split( '.' );
			var major = versionComponents.Length > 0 ? versionComponents[ 0 ] : "invalid";
			var minor = versionComponents.Length > 1 ? versionComponents[ 1 ] : "invalid";
			var patch = versionComponents.Length > 2 ? versionComponents[ 2 ] : "0";
			var preview = versionComponents.Length > 3 ? versionComponents[ 3 ] : string.Empty;

			var formattedVersion = $"{major}.{minor}.{patch}{preview}";
			return SupportedVersions.Contains( formattedVersion );
		}

		public static void RejectUnsupportedVersions( string version, int? lineNumber = null )
		{
			if ( string.IsNullOrEmpty( version ) )
			{
				ExceptionLogger.LogAndThrow( new VersionNotFoundException( $"Version cannot be blank." ), lineNumber );
			}

			if ( !IsVersionSupported( version ) )
			{
				ExceptionLogger.LogAndThrow( new VersionNotFoundException( $"Version \"{version}\" is not supported by {AppInfo.AppNameAndVersion}." ), lineNumber );
			}

			Log.Info( $"Version \"{version}\" is compatible with {AppInfo.AppNameAndVersion}." );
		}
	}
}
