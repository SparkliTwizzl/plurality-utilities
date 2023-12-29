using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptInputParserTests
	{
		public struct TestData
		{
			public static ShortcutScriptEntry[] Entries => new[]
			{
				new ShortcutScriptEntry( new() { new("name", "tag") }, "pronoun", "decorator" ),
			};
			public static int EntriesRegionLength => 15;
			public static ShortcutScriptInput Input => new( ModuleOptions, Entries, Templates, Macros );
			public static string[] Macros => new[]
			{
				"::tag::name pronoun decorator",
			};
			public static ShortcutScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFilePath, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static int ModuleOptionsRegionLength => 3;
			public static string[] Templates => new[]
			{
				"::`tag`::`name` `pronoun` `decorator`",
			};
			public static int TemplatesRegionLength => 3;
		}


		public class ShortcutScriptEntryParserStub : IShortcutScriptEntryParser
		{
			public ShortcutScriptEntry[] ParseEntriesFromData( string[] data, ref int i )
			{
				i += TestData.EntriesRegionLength;
				return TestData.Entries;
			}
		}

		public class ShortcutScriptModuleOptionsParserStub : IShortcutScriptModuleOptionsParser
		{
			public ShortcutScriptModuleOptions ParseModuleOptionsFromData( string[] data, ref int i )
			{
				i += TestData.ModuleOptionsRegionLength;
				return TestData.ModuleOptions;
			}
		}

		public class ShortcutScriptTemplateParserStub : IShortcutScriptTemplateParser
		{
			public string[] ParseTemplatesFromData( string[] data, ref int i )
			{
				i += TestData.TemplatesRegionLength;
				return TestData.Templates;
			}
		}


		public ShortcutScriptEntryParserStub? entryParserStub;
		public ShortcutScriptInputParser? inputParser;
		public Mock<IShortcutScriptMacroParser>? macroParserMock;
		public ShortcutScriptModuleOptionsParserStub? moduleOptionsParserStub;
		public ShortcutScriptTemplateParserStub? templateParserStub;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			entryParserStub = new();
			macroParserMock = new();
			_ = macroParserMock
				.Setup( x => x.GenerateMacrosFromInput( It.IsAny<ShortcutScriptInput>() ) )
				.Returns( TestData.Macros );
			moduleOptionsParserStub = new();
			templateParserStub = new();

			inputParser = new ShortcutScriptInputParser( moduleOptionsParserStub, entryParserStub, templateParserStub, macroParserMock.Object );
		}


		[TestMethod]
		[DataRow( "ShortcutScriptInputParser_Valid.txt" )]
		public void ParseInputFileTest_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.Info( data );
			var expected = TestData.Input;
			var actual = inputParser!.ParseInputFile( filePath );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( FileNotFoundException ) )]
		[DataRow( "nonexistent.txt" )]
		public void ParseInputFileTest_ThrowsFileNotFoundException( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			_ = inputParser!.ParseInputFile( filePath );
		}
	}
}
