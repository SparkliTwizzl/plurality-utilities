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
		public static string DevelopmentAppVersionSuffix => "-dev";
		public static string Major => "0";
		public static string Minor => "9";
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
		};
		public static string[] SupportedPreviewVersions => new[]
		{
			Preview,
		};
		public static string VersionWildcard => "*";



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
			var patch = versionComponents.Length > 2 ? versionComponents[ 2 ] : string.Empty;
			var preview = versionComponents.Length > 3 ? versionComponents[ 3 ] : string.Empty;

			var isMajorSupported = SupportedMajorVersions.Contains( major ) || SupportedMajorVersions.Contains( VersionWildcard );
			var isMinorSupported = SupportedMinorVersions.Contains( minor ) || SupportedMinorVersions.Contains( VersionWildcard );
			var isPatchSupported = SupportedPatchVersions.Contains( patch ) || SupportedPatchVersions.Contains( VersionWildcard );
			var isPreviewSupported = SupportedPreviewVersions.Contains( preview ) || SupportedPreviewVersions.Contains( VersionWildcard );
			return isMajorSupported && isMinorSupported && isPatchSupported && isPreviewSupported;
		}
	}
}
