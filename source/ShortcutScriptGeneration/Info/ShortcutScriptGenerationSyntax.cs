using Petrichor.Common.Info;


namespace Petrichor.ShortcutScriptGeneration.Info
{
	public struct ShortcutScriptGenerationSyntax
	{
		public static string DefaultIconFilePathToken => $"{DefaultIconFilePathTokenName}{CommonSyntax.TokenValueDivider}";
		public static string DefaultIconFilePathTokenName => "default-icon";
		public static string EntriesRegionToken => $"{EntriesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntriesRegionTokenName => "entries";
		public static string EntryDecorationToken => $"{EntryDecorationTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryDecorationTokenName => "decoration";
		public static string EntryNameToken => $"{EntryNameTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryNameTokenName => "name";
		public static string EntryPronounToken => $"{EntryPronounTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryPronounTokenName => "pronoun";
		public static string EntryTagToken => $"{EntryTagTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryTagTokenName => "tag";
		public static string EntryRegionToken => $"{EntryRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryRegionTokenName => "entry";
		public static string ModuleOptionsRegionToken => $"{ModuleOptionsRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string ModuleOptionsRegionTokenName => "module-options";
		public static string ReloadShortcutToken => $"{ReloadShortcutTokenName}{CommonSyntax.TokenValueDivider}";
		public static string ReloadShortcutTokenName => "reload-shortcut";
		public static string SuspendIconFilePathToken => $"{SuspendIconFilePathTokenName}{CommonSyntax.TokenValueDivider}";
		public static string SuspendIconFilePathTokenName => "suspend-icon";
		public static string SuspendShortcutToken => $"{SuspendShortcutTokenName}{CommonSyntax.TokenValueDivider}";
		public static string SuspendShortcutTokenName => "suspend-shortcut";
		public static string TemplateToken => $"{TemplateTokenName}{CommonSyntax.TokenValueDivider}";
		public static string TemplateTokenName => "template";
		public static string TemplateFindDecorationString => $"{TemplateFindStringOpenChar}{EntryDecorationTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindNameString => $"{TemplateFindStringOpenChar}{EntryNameTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindPronounString => $"{TemplateFindStringOpenChar}{EntryPronounTokenName}{TemplateFindStringCloseChar}";
		public static char TemplateFindStringCloseChar => ']';
		public static char TemplateFindStringOpenChar => '[';
		public static string TemplateFindTagString => $"{TemplateFindStringOpenChar}{EntryTagTokenName}{TemplateFindStringCloseChar}";
		public static string TemplatesRegionToken => $"{TemplatesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string TemplatesRegionTokenName => "templates";
	}
}
