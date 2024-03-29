namespace Petrichor.Common.Syntax
{
	public readonly struct ControlSequences
	{
		public const char Escape = '\\';
		public const string EscapeStandin = "&bksl";
		public const char RegionClose = '}';
		public const char RegionOpen = '{';
		public const char FindTagClose = ']';
		public const string FindTagCloseStandin = "&clsqb";
		public const char FindTagOpen = '[';
		public const string FindTagOpenStandin = "&opsqb";
		public const char TokenValueDivider = ':';
	}
}
