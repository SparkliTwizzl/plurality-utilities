namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public struct TemplateFindStrings
	{
		public static string Color => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.Color.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Decoration => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.Decoration.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string ID => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.ID.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Name => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.Name.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string LastName => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.LastName.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string LastTag => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.LastTag.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Pronoun => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.Pronoun.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
		public static string Tag => $"{Common.Syntax.OperatorChars.TokenNameOpen}{Tokens.Tag.Key}{Common.Syntax.OperatorChars.TokenNameClose}";
	}
}
