using Petrichor.Common.Info;


namespace Petrichor.ShortcutScriptGeneration.Info
{
	public struct ShortcutScriptGenerationSyntax
	{
		public static string DefaultIconFilePathToken => $"{DefaultIconFilePathTokenName}{CommonSyntax.TokenValueDivider}";
		public static string DefaultIconFilePathTokenName => "default-icon";
		public static string EntriesRegionToken => $"{EntriesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntriesRegionTokenName => "entries";
		public static string EntryColorToken => $"{EntryColorTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryColorTokenName => "color";
		public static string EntryDecorationToken => $"{EntryDecorationTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryDecorationTokenName => "decoration";
		public static string EntryIDToken => $"{EntryIDTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryIDTokenName => "id";
		public static string EntryNameToken => $"{EntryNameTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryNameTokenName => "name";
		public static string EntryLastNameToken => $"{EntryLastNameTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryLastNameTokenName => "last-name";
		public static string EntryLastTagToken => $"{EntryLastTagTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntryLastTagTokenName => "last-tag";
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
		public static string TemplateFindColorString => $"{TemplateFindStringOpenChar}{EntryColorTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindDecorationString => $"{TemplateFindStringOpenChar}{EntryDecorationTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindIDString => $"{TemplateFindStringOpenChar}{EntryIDTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindNameString => $"{TemplateFindStringOpenChar}{EntryNameTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindLastNameString => $"{TemplateFindStringOpenChar}{EntryLastNameTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindLastTagString => $"{TemplateFindStringOpenChar}{EntryLastTagTokenName}{TemplateFindStringCloseChar}";
		public static string TemplateFindPronounString => $"{TemplateFindStringOpenChar}{EntryPronounTokenName}{TemplateFindStringCloseChar}";
		public static char TemplateFindStringCloseChar => ']';
		public static char TemplateFindStringOpenChar => '[';
		public static string TemplateFindTagString => $"{TemplateFindStringOpenChar}{EntryTagTokenName}{TemplateFindStringCloseChar}";
		public static string TemplatesRegionToken => $"{TemplatesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string TemplatesRegionTokenName => "templates";
	}
}
