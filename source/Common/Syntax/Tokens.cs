namespace Petrichor.Common.Syntax
{
	public struct Tokens
	{
		public static string RegionClose => TokenNames.RegionClose;
		public static string LineComment => TokenNames.LineComment;
		public static string MetadataRegion => $"{ TokenNames.MetadataRegion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string MinimumVersion => $"{ TokenNames.MinimumVersion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string RegionOpen => TokenNames.RegionOpen;
	}
}
