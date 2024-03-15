using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using System.Data;

namespace Petrichor.Common.Info
{
	public static class AppVersion
	{
		private static string AnyVersion => "*";


		public static string Current =>
#if DEBUG
			DevelopmentAppVersion;
#else
			ReleaseAppVersion;
#endif
		public static string DevelopmentAppVersion => $"{ReleaseAppVersion}{DevelopmentAppVersionSuffix}";
		public static string DevelopmentAppVersionSuffix => "-dev";
		public static string Major => "0";
		public static string Minor => "10";
		public static string Patch => "0";
		public static string Preview => "";
		public static string ReleaseAppVersion => $"{Major}.{Minor}.{Patch}{Preview}";
		public static string[] SupportedMajorVersions => new[]
		{
			Major,
		};
		public static string[] SupportedMinorVersions => new[]
		{
			Minor,
		};
		public static string[] SupportedPatchVersions => new[]
		{
			Patch,
			AnyVersion,
		};
		public static string[] SupportedPreviewVersions => new[]
		{
			Preview,
			AnyVersion,
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
			var major = versionComponents.Length > 0 ? versionComponents[ 0 ] : string.Empty;
			var minor = versionComponents.Length > 1 ? versionComponents[ 1 ] : string.Empty;
			var patch = versionComponents.Length > 2 ? versionComponents[ 2 ] : AnyVersion;
			var preview = versionComponents.Length > 3 ? versionComponents[ 3 ] : AnyVersion;

			var isMajorSupported = SupportedMajorVersions.Contains( major );
			var isMinorSupported = SupportedMinorVersions.Contains( minor );
			var isPatchSupported = SupportedPatchVersions.Contains( patch );
			var isPreviewSupported = SupportedPreviewVersions.Contains( preview );
			return isMajorSupported && isMinorSupported && isPatchSupported && isPreviewSupported;
		}

		public static void RejectUnsupportedVersions( string version )
		{
			if ( string.IsNullOrEmpty( version ) )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"Input file version cannot be blank" ) );
			}

			if ( !IsVersionSupported( version ) )
			{
				ExceptionLogger.LogAndThrow( new VersionNotFoundException( $"Input file version ({ version }) is not supported by this version of { AppInfo.AppName }" ) );
			}
		}
	}
}
