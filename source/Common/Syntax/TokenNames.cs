namespace Petrichor.Common.Syntax
{
	public struct TokenNames
	{
		public const string LineComment = "//";
		public const string MetadataRegion = "metadata";
		public const string MinimumVersion = "minimum-version";
		public static string RegionClose => OperatorChars.RegionClose.ToString();
		public static string RegionOpen => OperatorChars.RegionOpen.ToString();
	}
}
