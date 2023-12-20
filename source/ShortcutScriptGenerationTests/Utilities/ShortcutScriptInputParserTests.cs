using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Utilities;
using Petrichor.TestShared.Info;


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
			public static ShortcutScriptInput Input => new( Metadata!, Entries!, Templates! );
			public static ShortcutScriptMetadata Metadata => new( TestAssets.DefaultIconFileName );
			public static string[] Templates => Array.Empty<string>();
		}


		public ShortcutScriptEntryParser? entryParser;
		public ShortcutScriptInputParser? inputParser;
		public ShortcutScriptMacroParser? macroParser;
		public ShortcutScriptMetadataParser? metadataParser;
		public ShortcutScriptTemplateParser? templateParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			metadataParser = new();
			entryParser = new();
			templateParser = new();
			macroParser = new();
			inputParser = new( metadataParser, entryParser, templateParser, macroParser );
		}


		[ TestMethod ]
		[ DataRow( "ShortcutScriptInputParser_Valid.txt" ) ]
		public void ParseInputFileTest_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.Info( data );
			var expected = TestData.Input;
			var actual = inputParser!.ParseInputFile( filePath );
			Assert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( FileNotFoundException) ) ]
		[ DataRow( "nonexistent.txt" ) ]
		public void ParseInputFileTest_ThrowsFileNotFoundException( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			_ = inputParser!.ParseInputFile( filePath );
		}
	}
}
