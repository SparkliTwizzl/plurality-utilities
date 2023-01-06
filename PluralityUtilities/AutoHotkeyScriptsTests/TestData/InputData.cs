using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;


namespace PluralityUtilities.AutoHotkeyScriptsTests.TestData
{
	public static class InputData
	{
		public static readonly Person[] AutoHotkeyScriptGenerator_ValidPeople = ExpectedOutputData.ParsedInputData;
		public static readonly string[] AutoHotkeyScriptGenerator_MacroTemplates = ExpectedOutputData.CreatedMacroData;
		public static readonly string[] TemplateParser_ValidRawTemplates = ExpectedOutputData.ParsedTemplateData;
	}
}
