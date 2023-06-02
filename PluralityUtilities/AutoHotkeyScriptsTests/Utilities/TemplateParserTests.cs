using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.AutoHotkeyScriptsTests.TestData;
using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class TemplateParserTests
	{
		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void CreateMacrosFromTemplatesTest_Success()
		{
			var entries = InputData.TemplateParserData.ValidEntries;
			var templates = InputData.TemplateParserData.ValidTemplates;
			var results = TemplateParser.CreateMacrosFromInput( entries, templates );
			var expected = ExpectedOutputData.GeneratedMacros;
			var actual = results.ToArray();
			Log.WriteLine( "expected:" );
			foreach ( var line in expected )
			{
				Log.WriteLine( $"[{ line }]" );
			}
			Log.WriteLine();
			Log.WriteLine( "actual:" );
			foreach ( var line in actual )
			{
				Log.WriteLine( $"[{ line }]" );
			}
			CollectionAssert.AreEqual( expected, actual );
		}

		//[ TestMethod ]
		//[ DataRow( "TemplateParser_ValidTemplates.txt" ) ]
		//public void ParseTemplatesFromFileTest_Success( string inputFile )
		//{
		//	var filePath = TestUtilities.LocateInputFile( inputFile );
		//	var expected = ExpectedOutputData.ParsedTemplates;
		//	var actual = TemplateParser.ParseTemplatesFromFile( filePath );
		//	CollectionAssert.AreEqual( expected, actual );
		//}

		//[ TestMethod ]
		//[ ExpectedException( typeof( EscapeCharacterMismatchException ) ) ]
		//[ DataRow( "TemplateParser_TrailingEscapeCharacter.txt" ) ]
		//public void ParseTemplatesFromFileTest_ThrowsEscapeCharacterMismatchException( string inputFile )
		//{
		//	var filePath = TestUtilities.LocateInputFile( inputFile );
		//	_ = TemplateParser.ParseTemplatesFromFile( filePath );
		//}
	}
}