using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.TestCommon;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{

	[ TestClass ]
	public class AutoHotkeyScriptGeneratorTests
	{
		public static class TestData
		{
			public static readonly string[] Macros = new string[]
			{
				"::@tag::name pronoun decoration",
			};
			public static readonly string[] GeneratedOutputFileContents = new string[]
			{
				"#SingleInstance Force",
				"",
				"::@tag::name pronoun decoration",
			};
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
			AutoHotkeyScriptGenerator.GenerateScript( TestData.Macros, outputFile );
			var expected = TestData.GeneratedOutputFileContents;
			var actual = File.ReadAllLines( outputFile );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}