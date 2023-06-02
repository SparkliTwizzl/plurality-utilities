using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.AutoHotkeyScriptsTests.TestData;
using PluralityUtilities.TestCommon;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class AutoHotkeyScriptGeneratorTests
	{
		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void GenerateScriptTest_Success()
		{
			var outputFile = $"{ TestDirectories.TestOutputDir }{ nameof( AutoHotkeyScriptGenerator ) }_{ nameof( GenerateScriptTest_Success ) }.ahk";
			AutoHotkeyScriptGenerator.GenerateScript( InputData.AutoHotkeyScriptGeneratorData.ValidMacroTemplates, outputFile );
			var expected = ExpectedOutputData.GeneratedOutputFileContents;
			var actual = File.ReadAllLines( outputFile );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}