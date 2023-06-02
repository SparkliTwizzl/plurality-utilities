using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.TestCommon;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{

	[ TestClass ]
	public class AutoHotkeyScriptGeneratorTests
	{
		public static class InputData
		{
			public static readonly string[] ValidMacroTemplates = SharedExpectedOutputData.GeneratedMacros;
		}


		[TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void GenerateScriptTest_Success()
		{
			var outputFile = $"{ TestDirectories.TestOutputDir }{ nameof( AutoHotkeyScriptGenerator ) }_{ nameof( GenerateScriptTest_Success ) }.ahk";
			AutoHotkeyScriptGenerator.GenerateScript( InputData.ValidMacroTemplates, outputFile );
			var expected = SharedExpectedOutputData.GeneratedOutputFileContents;
			var actual = File.ReadAllLines( outputFile );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}