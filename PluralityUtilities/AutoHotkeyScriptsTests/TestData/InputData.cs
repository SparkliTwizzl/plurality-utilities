using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;


namespace PluralityUtilities.AutoHotkeyScriptsTests.TestData
{
	public static class InputData
	{
		public static class AutoHotkeyScriptGeneratorData
		{
			public static readonly string[] ValidMacroTemplates = ExpectedOutputData.CreatedMacroData;
		}

		public static class TemplateParserData
		{
			public static readonly Person[] ValidPeople = ExpectedOutputData.ParsedInputData;
			public static readonly string[] ValidTemplates = ExpectedOutputData.ParsedTemplateData;
		}
}
}
