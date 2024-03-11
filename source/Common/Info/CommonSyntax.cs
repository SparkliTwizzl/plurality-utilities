namespace Petrichor.Common.Info
{
	public struct CommonSyntax
	{ 
		public static string CloseBracketToken => CloseBracketTokenName;
		public const string CloseBracketTokenName = "}";
		public const char EscapeChar = '\\';
		public const string EscapeCharStandin = "&bksl";
		public const char FindTokenCloseChar = ']';
		public const string FindTokenCloseCharStandin = "&clsqb";
		public const char FindTokenOpenChar = '[';
		public const string FindTokenOpenCharStandin = "&opsqb";
		public static string LineCommentToken => $"{ LineCommentTokenName }{ TokenValueDivider }";
		public const string LineCommentTokenName = "#";
		public static string MetadataRegionToken => $"{ MetadataRegionTokenName }{ TokenValueDivider }";
		public const string MetadataRegionTokenName = "metadata";
		public static string MinimumVersionToken => $"{ MinimumVersionTokenName }{ TokenValueDivider }";
		public const string MinimumVersionTokenName = "minimum-version";
		public static string OpenBracketToken => OpenBracketTokenName;
		public const string OpenBracketTokenName = "{";
		public const string TokenValueDivider = ":";
	 }
 }
