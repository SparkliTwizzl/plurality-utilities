using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;


namespace PluralityUtilities.AutoHotkeyScriptsTests.TestData
{
	public static class InputData
	{
		public static class AutoHotkeyScriptGeneratorData
		{
			public static readonly string[] ValidMacroTemplates = ExpectedOutputData.GeneratedMacros;
		}

		public static class TemplateParserData
		{
			public static readonly Entry[] ValidEntries = ExpectedOutputData.ParsedEntries;
			public static readonly string[] ValidTemplates = ExpectedOutputData.ParsedTemplates;
		}
}
}
