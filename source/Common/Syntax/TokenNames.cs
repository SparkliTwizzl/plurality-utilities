namespace Petrichor.Common.Syntax
{
	public struct TokenNames
	{
		public const string LineComment = "//";
		public const string MetadataRegion = "metadata";
		public const string MinimumVersion = "minimum-version";
		public static string RegionClose => $"{ Common.Syntax.OperatorChars.RegionClose }";
		public static string RegionOpen => $"{ Common.Syntax.OperatorChars.RegionOpen }";
	}
}
