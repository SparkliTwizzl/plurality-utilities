using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Common.Containers;
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
				new ScriptEntry(),
			};
			public const int EntriesRegionLength = 3;
			public static ScriptInput Input => new( ModuleOptions, Entries, Templates, Macros );
			public static string[] Macros => new[]
			{
				"macro",
			};
			public const string MetadataRegionIsValidMessage = "mock value";
			public const int MetadataRegionLength = 3;
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFilePath, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public const int ModuleOptionsRegionLength = 3;
			public static string[] Templates => new[]
			{
				"template",
			};
			public const int TemplatesRegionLength = 3;
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

		public class RegionParserStub<T> : IRegionParser<T> where T : new()
		{

			public bool HasParsedMaxAllowedRegions { get; set; } = false;
			public int LinesInRegion { get; set; } = 0;
			public int LinesParsed { get; set; } = 0;
			public int MaxRegionsAllowed { get; set; } = 0;
			public string RegionName { get; set; } = string.Empty;
			public int RegionsParsed { get; set; } = 0;
			public T ReturnValue { get; set; } = new();

			public T Parse( string[] regionData )
			{
				++RegionsParsed;
				HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;
				LinesParsed = LinesInRegion;
				return ReturnValue;
			}
		}

		public RegionParserStub< List< ScriptEntry > >? entriesRegionParserStub;
		public InputFileParser? parser;
		public Mock<IMacroGenerator>? macroGeneratorMock;
		public RegionParserStub< StringWrapper >? metadataRegionParserStub;
		public RegionParserStub< ScriptModuleOptions >? moduleOptionsRegionParserStub;
		public RegionParserStub< List< string > >? templatesRegionParserStub;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			entriesRegionParserStub = new()
			{
				LinesInRegion = TestData.EntriesRegionLength,
				MaxRegionsAllowed = Info.DataRegionInfo.EntriesRegionsAllowed,
				ReturnValue = TestData.Entries.ToList(),
			};

			macroGeneratorMock = new();
			_ = macroGeneratorMock
				.Setup( x => x.Generate( It.IsAny<ScriptInput>() ) )
				.Returns( TestData.Macros );

			metadataRegionParserStub = new()
			{
				LinesInRegion = TestData.MetadataRegionLength,
				MaxRegionsAllowed = Common.Info.DataRegionInfo.MetadataRegionsAllowed,
				ReturnValue = new StringWrapper( TestData.MetadataRegionIsValidMessage ),
			};

			moduleOptionsRegionParserStub = new()
			{
				LinesInRegion = TestData.ModuleOptionsRegionLength,
				MaxRegionsAllowed = Info.DataRegionInfo.ModuleOptionsRegionsAllowed,
				ReturnValue = TestData.ModuleOptions,
			};

			templatesRegionParserStub = new()
			{
				LinesInRegion = TestData.TemplatesRegionLength,
				MaxRegionsAllowed = Info.DataRegionInfo.TemplatesRegionsAllowed,
				ReturnValue = TestData.Templates.ToList(),
			};

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
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void ParseFile_Test_Throws_BracketException( string fileName ) => _ = parser!.Parse( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketException_Data
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
