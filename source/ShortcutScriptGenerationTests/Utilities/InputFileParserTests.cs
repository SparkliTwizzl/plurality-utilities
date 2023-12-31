using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
			public static ScriptEntry[] Entries => new[]
			{
				new ScriptEntry( new() { new("name", "tag") }, "pronoun", "decorator" ),
			};
			public static int EntriesRegionLength => 3;
			public static ScriptInput Input => new( ModuleOptions, Entries, Templates, Macros );
			public static string[] Macros => new[]
			{
				"::tag::name pronoun decorator",
			};
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFilePath, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static int ModuleOptionsRegionLength => 3;
			public static string[] Templates => new[]
			{
				"::`tag`::`name` `pronoun` `decorator`",
			};
			public static int TemplatesRegionLength => 3;
		}


		public class EntriesRegionParserStub : IEntriesRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public int RegionsParsed { get; private set; } = 0;

			public ScriptEntry[] Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = true;
				LinesParsed = TestData.EntriesRegionLength;
				return TestData.Entries;
			}
		}

		public class ModuleOptionsRegionParserStub : IModuleOptionsRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public int RegionsParsed { get; private set; } = 0;

			public ScriptModuleOptions Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = true;
				LinesParsed = TestData.ModuleOptionsRegionLength;
				return TestData.ModuleOptions;
			}
		}

		public class TemplatesRegionParserStub : ITemplatesRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public int RegionsParsed { get; private set; } = 0;

			public string[] Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = true;
				LinesParsed = TestData.TemplatesRegionLength;
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
				.Setup( x => x.Generate( It.IsAny<ScriptInput>() ) )
				.Returns( TestData.Macros );
			moduleOptionsRegionParserStub = new();
			templatesRegionParserStub = new();

			inputFileParser = new InputFileParser( moduleOptionsRegionParserStub, entriesRegionParserStub, templatesRegionParserStub, macroGeneratorMock.Object );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string fileName )
		{
			var expected = TestData.Input;
			var filePath = TestUtilities.LocateInputFile( fileName );
			var actual = inputFileParser!.Parse( filePath );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputFileParser )}_{nameof( Parse_Test_Success )}.petrichor" };
			}
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
