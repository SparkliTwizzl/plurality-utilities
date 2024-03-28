namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public readonly struct TemplateTextCases
	{
		public const string Default = Unchanged;
		public const string FirstCaps = "firstCaps";
		public const string Lower = "lower";
		public const string Unchanged = "unchanged";
		public const string Upper = "upper";

		public static string[] LookUpTable => new[]
		{
			Default,
			FirstCaps,
			Lower,
			Unchanged,
			Upper,
		};
	}
}
