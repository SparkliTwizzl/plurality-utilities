using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
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
			generator.GenerateScript(ValidData.expectedValidInputData.ToList(), outputFile);
			var expected = ValidData.expectedValidOutputData;
			var actual = File.ReadAllLines(outputFile);
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}