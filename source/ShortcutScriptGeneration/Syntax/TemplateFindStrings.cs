namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public readonly struct TemplateFindStrings
	{
		public static string Color => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.Color.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string Decoration => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.Decoration.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string ID => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.ID.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string Name => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.Name.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string LastName => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.LastName.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string LastTag => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.LastTag.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string Pronoun => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.Pronoun.Key}{Common.Syntax.ControlSequences.FindTagClose}";
		public static string Tag => $"{Common.Syntax.ControlSequences.FindTagOpen}{Tokens.Tag.Key}{Common.Syntax.ControlSequences.FindTagClose}";
	}
}
