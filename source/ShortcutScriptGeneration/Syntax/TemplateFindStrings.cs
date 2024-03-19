namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public struct TemplateFindStrings
	{
		public static string Color => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryColor}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Decoration => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryDecoration}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string ID => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryID}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Name => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryName}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string LastName => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryLastName}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string LastTag => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryLastTag}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Pronoun => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryPronoun}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Tag => $"{Common.Syntax.OperatorChars.TokenNameOpen}{TokenNames.EntryTag}{Common.Syntax.OperatorChars.TokenNameClose}";
	}
}
