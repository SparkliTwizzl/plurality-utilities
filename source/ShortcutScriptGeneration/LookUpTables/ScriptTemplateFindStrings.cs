using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.LookUpTables
{
	public static class ScriptTemplateFindStrings
	{
		public static string[] LookUpTable => new[]
		{
			ShortcutScriptGenerationSyntax.TemplateFindDecorationString,
			ShortcutScriptGenerationSyntax.TemplateFindNameString,
			ShortcutScriptGenerationSyntax.TemplateFindPronounString,
			ShortcutScriptGenerationSyntax.TemplateFindTagString,
		};
	}
}
