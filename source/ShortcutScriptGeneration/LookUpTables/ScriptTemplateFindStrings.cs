using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.LookUpTables
{
	public static class ScriptTemplateFindStrings
	{
		public static string[] LookUpTable => new[]
		{
			ShortcutScriptSyntax.TemplateFindColorString,
			ShortcutScriptSyntax.TemplateFindDecorationString,
			ShortcutScriptSyntax.TemplateFindIDString,
			ShortcutScriptSyntax.TemplateFindNameString,
			ShortcutScriptSyntax.TemplateFindLastNameString,
			ShortcutScriptSyntax.TemplateFindLastTagString,
			ShortcutScriptSyntax.TemplateFindPronounString,
			ShortcutScriptSyntax.TemplateFindTagString,
		};
	}
}
