using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class InputParserTests
	{
		public static class TestData
		{
			public static Entry[] Entries = new Entry[]
					{
						new Entry
						(
							new List<Identity>
							{
								new Identity("name", "tag")
							},
							"",
							""
						),
					};
			public static string[] Templates = new string[] { };
			public static Input ParsedInput = new Input( Entries, Templates );
		}


		public int i;
		public EntryParser? EntryParser;
		public InputParser? InputParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			EntryParser = new EntryParser();
			InputParser = new InputParser( EntryParser );
		}


		[ TestMethod ]
		[ DataRow( "InputParser_Valid.txt" ) ]
		public void ParseInputFileTest_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.WriteLineTimestamped( data );
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
