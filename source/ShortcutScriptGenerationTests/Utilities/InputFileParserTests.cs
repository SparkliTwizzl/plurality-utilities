using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class InputFileParserTests
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


		public class EntriesRegionParserStub : IEntriesRegionParser
		{
			public ShortcutScriptEntry[] ParseEntriesFromData( string[] data, ref int i )
			{
				i += TestData.EntriesRegionLength;
				return TestData.Entries;
			}
		}

		public class ModuleOptionsRegionParserStub : IModuleOptionsRegionParser
		{
			public ShortcutScriptModuleOptions ParseModuleOptionsFromData( string[] data, ref int i )
			{
				i += TestData.ModuleOptionsRegionLength;
				return TestData.ModuleOptions;
			}
		}

		public class TemplatesRegionParserStub : ITemplatesRegionParser
		{
			public string[] ParseTemplatesFromData( string[] data, ref int i )
			{
				i += TestData.TemplatesRegionLength;
				return TestData.Templates;
			}
		}


		public EntriesRegionParserStub? entriesRegionParserStub;
		public InputFileParser? inputFileParser;
		public Mock<IMacroGenerator>? macroGeneratorMock;
		public ModuleOptionsRegionParserStub? moduleOptionsRegionParserStub;
		public TemplatesRegionParserStub? templatesRegionParserStub;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			entriesRegionParserStub = new();
			macroGeneratorMock = new();
			_ = macroGeneratorMock
				.Setup( x => x.Generate( It.IsAny<ShortcutScriptInput>() ) )
				.Returns( TestData.Macros );
			moduleOptionsRegionParserStub = new();
			templatesRegionParserStub = new();

			inputFileParser = new InputFileParser( moduleOptionsRegionParserStub, entriesRegionParserStub, templatesRegionParserStub, macroGeneratorMock.Object );
		}


		[TestMethod]
		[DataRow( "ShortcutScriptInputParser_Valid.txt" )]
		public void ParseFile_Test_Success( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			var data = File.ReadAllText( filePath );
			Log.Info( data );
			var expected = TestData.Input;
			var actual = inputFileParser!.Parse( filePath );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( FileNotFoundException ) )]
		[DataRow( "nonexistent.txt" )]
		public void ParseFile_Test_ThrowsFileNotFoundException( string fileName )
		{
			var filePath = TestUtilities.LocateInputFile( fileName );
			_ = inputFileParser!.Parse( filePath );
		}
	}
}
