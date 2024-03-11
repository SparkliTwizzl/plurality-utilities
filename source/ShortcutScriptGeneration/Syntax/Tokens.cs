using Petrichor.Common.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public struct Tokens
	{
		public static string DefaultIconFilePath => $"{ TokenNames.DefaultIconFilePath }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntriesRegion => $"{ TokenNames.EntriesRegion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryColor => $"{ TokenNames.EntryColor }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryDecoration => $"{ TokenNames.EntryDecoration }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryID => $"{ TokenNames.EntryID }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryName => $"{ TokenNames.EntryName }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryLastName => $"{ TokenNames.EntryLastName }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryLastTag => $"{ TokenNames.EntryLastTag }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryPronoun => $"{ TokenNames.EntryPronoun }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryTag => $"{ TokenNames.EntryTag }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string EntryRegion => $"{ TokenNames.EntryRegion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string ModuleOptionsRegion => $"{ TokenNames.ModuleOptionsRegion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string ReloadShortcut => $"{ TokenNames.ReloadShortcut }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string SuspendIconFilePath => $"{ TokenNames.SuspendIconFilePath }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string SuspendShortcut => $"{ TokenNames.SuspendShortcut }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string Template => $"{ TokenNames.Template }{ Common.Syntax.OperatorChars.TokenValueDivider }";
		public static string TemplatesRegion => $"{ TokenNames.TemplatesRegion }{ Common.Syntax.OperatorChars.TokenValueDivider }";
	}
}
