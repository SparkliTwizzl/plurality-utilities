namespace Petrichor.Common.Info
{
	public struct CommonSyntax
	{
		public static string CloseBracketToken => CloseBracketTokenName;
		public static string CloseBracketTokenName => "}";
		public static string LineCommentToken => $"{LineCommentTokenName}{TokenValueDivider}";
		public static string LineCommentTokenName => "#";
		public static string MetadataRegionToken => $"{MetadataRegionTokenName}{TokenValueDivider}";
		public static string MetadataRegionTokenName => "metadata";
		public static string MinimumVersionToken => $"{MinimumVersionTokenName}{TokenValueDivider}";
		public static string MinimumVersionTokenName => "minimum-version";
		public static string OpenBracketToken => OpenBracketTokenName;
		public static string OpenBracketTokenName => "{";
		public static string TokenValueDivider => ":";
	}
}
