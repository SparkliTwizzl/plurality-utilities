using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.Logging;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[ TestClass ]
	public class ShortcutScriptInputParserTests
	{
		public struct TestData
		{
			public static ShortcutScriptEntry[] Entries => new[]
			{
				new ShortcutScriptEntry( new() { new("name", "tag") }, "pronoun", "decorator" ),
			};
			public static ShortcutScriptInput Input => new( Entries!, Metadata!, Templates! );
			public static ShortcutScriptMetadata Metadata => new("./IconDefault.ico");
			public static string[] Templates => Array.Empty<string>();
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
		[ DataRow( "ShortcutScriptInputParser_Valid.txt" ) ]
		public void ParseInputFileTest_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.WriteLine( data );
			var expected = TestData.Input;
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
