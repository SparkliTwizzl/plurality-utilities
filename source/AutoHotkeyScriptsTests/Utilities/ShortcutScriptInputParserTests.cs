using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.AutoHotkeyScripts.Containers;
using Petrichor.Logging;
using Petrichor.TestShared.Utilities;


namespace Petrichor.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class ShortcutScriptInputParserTests
	{
		public static class TestData
		{
			public static ShortcutScriptEntry[] Entries = new ShortcutScriptEntry[]
					{
						new ShortcutScriptEntry
						(
							new List<ShortcutScriptIdentity>
							{
								new ShortcutScriptIdentity("name", "tag")
							},
							"",
							""
						),
					};
			public static string[] Templates = new string[] { };
			public static ShortcutScriptInput ParsedInput = new Input( Entries, Templates );
		}


		public ShortcutScriptEntryParser? EntryParser;
		public ShortcutScriptInputParser? InputParser;
		public ShortcutScriptTemplateParser? TemplateParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			EntryParser = new ShortcutScriptEntryParser();
			TemplateParser = new ShortcutScriptTemplateParser();
			InputParser = new ShortcutScriptInputParser( EntryParser, TemplateParser );
		}


		[ TestMethod ]
		[ DataRow( "InputParser_Valid.txt" ) ]
		public void ParseInputFileTest_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.WriteLine( data );
			var expected = TestData.ParsedInput;
			var actual = InputParser.ParseInputFile( filePath );
			Assert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( FileNotFoundException) ) ]
		[ DataRow( "nonexistent.txt" ) ]
		public void ParseInputFileTest_ThrowsFileNotFoundException( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			_ = InputParser.ParseInputFile( filePath );
		}
	}
}
