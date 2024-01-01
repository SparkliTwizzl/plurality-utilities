using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
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
			public static int MetadataRegionLength => 3;
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

		public class MetadataRegionParserStub : IMetadataRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public static string RegionIsValidMessage => Common.Utilities.MetadataRegionParser.RegionIsValidMessage;
			public int RegionsParsed { get; private set; } = 0;

			public string Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = true;
				LinesParsed = TestData.MetadataRegionLength;
				return RegionIsValidMessage;
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
			public int TemplatesParsed { get; private set; } = 0;

			public string[] Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = true;
				LinesParsed = TestData.TemplatesRegionLength;
				TemplatesParsed = 1;
				return TestData.Templates;
			}
		}


		public EntriesRegionParserStub? entriesRegionParserStub;
		public InputFileParser? parser;
		public Mock<IMacroGenerator>? macroGeneratorMock;
		public MetadataRegionParserStub? metadataRegionParserStub;
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
			metadataRegionParserStub = new();
			moduleOptionsRegionParserStub = new();
			templatesRegionParserStub = new();

			parser = new InputFileParser( metadataRegionParserStub, moduleOptionsRegionParserStub, entriesRegionParserStub, templatesRegionParserStub, macroGeneratorMock.Object );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string fileName )
		{
			var expected = TestData.Input;
			var filePath = TestUtilities.LocateInputFile( fileName );
			var actual = parser!.Parse( filePath );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputFileParser )}_Valid.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void ParseFile_Test_BracketMismatcheRegionException( string fileName ) => _ = parser!.Parse( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketMismatchException_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputFileParser )}_DanglingCloseBracket.petrichor" };
				yield return new object[] { $"{nameof( InputFileParser )}_DanglingOpenBracket.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileNotFoundException ) )]
		[DynamicData( nameof( Parse_Test_Throws_FileNotFoundException_Data ), DynamicDataSourceType.Property )]
		public void ParseFile_Test_Throws_FileNotFoundException( string fileName ) => _ = parser!.Parse( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> Parse_Test_Throws_FileNotFoundException_Data
		{
			get
			{
				yield return new object[] { "nonexistent.txt" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		[DynamicData( nameof( Parse_Test_Throws_FileRegionException_Data ), DynamicDataSourceType.Property )]
		public void ParseFile_Test_Throws_FileRegionException( string fileName ) => _ = parser!.Parse( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> Parse_Test_Throws_FileRegionException_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputFileParser )}_NoMetadataRegion.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void ParseFile_Test_Throws_TokenException( string fileName ) => _ = parser!.Parse( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputFileParser )}_UnknownToken.petrichor" };
			}
		}
	}
}
