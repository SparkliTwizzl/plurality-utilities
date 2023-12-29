using Petrichor.Common.Info;


namespace Petrichor.ShortcutScriptGeneration.Info
{
	public struct ShortcutScriptGenerationSyntax
	{
		public static string DefaultIconFilePathToken => $"{DefaultIconFilePathTokenName}{CommonSyntax.TokenValueDivider}";
		public static string DefaultIconFilePathTokenName => "default-icon";
		public static string EntriesRegionToken => $"{EntriesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string EntriesRegionTokenName => "entries";
		public static string ModuleOptionsRegionToken => $"{ModuleOptionsRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string ModuleOptionsRegionTokenName => "module-options";
		public static string ReloadShortcutToken => $"{ReloadShortcutTokenName}{CommonSyntax.TokenValueDivider}";
		public static string ReloadShortcutTokenName => "reload-shortcut";
		public static string SuspendIconFilePathToken => $"{SuspendIconFilePathTokenName}{CommonSyntax.TokenValueDivider}";
		public static string SuspendIconFilePathTokenName => "suspend-icon";
		public static string SuspendShortcutToken => $"{SuspendShortcutTokenName}{CommonSyntax.TokenValueDivider}";
		public static string SuspendShortcutTokenName => "suspend-shortcut";
		public static string TemplatesRegionToken => $"{TemplatesRegionTokenName}{CommonSyntax.TokenValueDivider}";
		public static string TemplatesRegionTokenName => "templates";
	}
}
