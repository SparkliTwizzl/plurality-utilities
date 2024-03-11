using Petrichor.Common.Info;


namespace Petrichor.ShortcutScriptGeneration.Info
{
	public struct ShortcutScriptSyntax
	{ 
		public static string DefaultIconFilePathToken
			=> $"{ DefaultIconFilePathTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string DefaultIconFilePathTokenName = "default-icon";
		public static string EntriesRegionToken
			=> $"{ EntriesRegionTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntriesRegionTokenName = "entries";
		public static string EntryColorToken
			=> $"{ EntryColorTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryColorTokenName = "color";
		public static string EntryDecorationToken
			=> $"{ EntryDecorationTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryDecorationTokenName = "decoration";
		public static string EntryIDToken
			=> $"{ EntryIDTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryIDTokenName = "id";
		public static string EntryNameToken
			=> $"{ EntryNameTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryNameTokenName = "name";
		public static string EntryLastNameToken
			=> $"{ EntryLastNameTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryLastNameTokenName = "last-name";
		public static string EntryLastTagToken
			=> $"{ EntryLastTagTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryLastTagTokenName = "last-tag";
		public static string EntryPronounToken
			=> $"{ EntryPronounTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryPronounTokenName = "pronoun";
		public static string EntryTagToken
			=> $"{ EntryTagTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryTagTokenName = "tag";
		public static string EntryRegionToken
			=> $"{ EntryRegionTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string EntryRegionTokenName = "entry";
		public static string ModuleOptionsRegionToken
			=> $"{ ModuleOptionsRegionTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string ModuleOptionsRegionTokenName = "module-options";
		public static string ReloadShortcutToken
			=> $"{ ReloadShortcutTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string ReloadShortcutTokenName = "reload-shortcut";
		public static string SuspendIconFilePathToken
			=> $"{ SuspendIconFilePathTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string SuspendIconFilePathTokenName = "suspend-icon";
		public static string SuspendShortcutToken
			=> $"{ SuspendShortcutTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string SuspendShortcutTokenName = "suspend-shortcut";
		public static string TemplateToken
			=> $"{ TemplateTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string TemplateTokenName = "template";
		public static string TemplateFindColorString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryColorTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindDecorationString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryDecorationTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindIDString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryIDTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindNameString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryNameTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindLastNameString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryLastNameTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindLastTagString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryLastTagTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindPronounString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryPronounTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplateFindTagString
			=> $"{ CommonSyntax.FindTokenOpenChar }{ EntryTagTokenName }{ CommonSyntax.FindTokenCloseChar }";
		public static string TemplatesRegionToken
			=> $"{ TemplatesRegionTokenName }{ CommonSyntax.TokenValueDivider }";
		public const string TemplatesRegionTokenName = "templates";
	 }
 }
