using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon;
using PluralityUtilities.TestCommon.TestData;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class AutoHotkeyScriptGeneratorTests
	{
		public AutoHotkeyScriptGenerator generator = new AutoHotkeyScriptGenerator();


		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
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