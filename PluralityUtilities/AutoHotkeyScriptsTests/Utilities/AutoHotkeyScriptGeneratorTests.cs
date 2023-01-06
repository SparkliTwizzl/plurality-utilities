using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.AutoHotkeyScriptsTests.TestData;
using PluralityUtilities.TestCommon;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class AutoHotkeyScriptGeneratorTests
	{
		public AutoHotkeyScriptGenerator generator = new AutoHotkeyScriptGenerator();


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[TestMethod]
		public void GenerateScriptTest_Success()
		{
			var outputFile = $"{TestDirectories.TestOutputDir}{nameof(AutoHotkeyScriptGenerator)}_{nameof(GenerateScriptTest_Success)}.ahk";
			generator.GenerateScript(InputData.AutoHotkeyScriptGenerator_Valid.ToList(), outputFile);
			var expected = ExpectedOutputData.GeneratedOutputData;
			var actual = File.ReadAllLines(outputFile);
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}