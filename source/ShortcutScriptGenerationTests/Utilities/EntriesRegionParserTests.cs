using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class EntriesRegionParserTests
	{
		public struct TestData
		{
			public static ScriptEntry[] Entries => new[]
			{
				new ScriptEntry(),
				new ScriptEntry(),
			};
			public static int EntryRegionLength => 3;
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
			};
			public static string[] RegionData_UnknownToken => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\tunknown{ Common.Syntax.OperatorChars.TokenValueDivider } token",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_Valid => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Common.Syntax.Tokens.LineComment } line comment",
				$"\t{ Syntax.Tokens.EntryRegion } { Common.Syntax.Tokens.LineComment } inline comment",
				$"\t{ Common.Syntax.Tokens.RegionOpen }",
				$"\t\t{ Common.Syntax.Tokens.LineComment } entry region body",
				$"\t{ Common.Syntax.Tokens.RegionClose }",
				"\t",
				$"\t{ Common.Syntax.Tokens.LineComment } line comment",
				$"\t{ Syntax.Tokens.EntryRegion }",
				$"\t{ Common.Syntax.Tokens.RegionOpen }",
				$"\t\t{ Common.Syntax.Tokens.LineComment } entry region body",
				$"\t{ Common.Syntax.Tokens.RegionClose }",
				Common.Syntax.Tokens.RegionClose,
			};
		}


		public class EntryParserStub : IEntryRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public int RegionsParsed { get; private set; } = 0;

			public ScriptEntry Parse( string[] regionData )
			{
				++RegionsParsed;
				LinesParsed = TestData.EntryRegionLength;
				return new ScriptEntry();
			}
		}


		public EntryParserStub? entryRegionParserStub;
		public EntriesRegionParser? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			entryRegionParserStub = new();
			parser = new( entryRegionParserStub );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expectedResult = TestData.Entries;
			var actualResult = parser!.Parse( regionData );
			CollectionAssert.AreEqual( expectedResult, actualResult );
			
			var expectedHasParsedMaxAllowedRegions = true;
			var actualHasParsedMaxAllowedRegions = parser.HasParsedMaxAllowedRegions;
			Assert.AreEqual( expectedHasParsedMaxAllowedRegions, actualHasParsedMaxAllowedRegions );

			var expectedLinesParsed = regionData.Length;
			var actualLinesParsed = parser.LinesParsed;
			Assert.AreEqual( expectedLinesParsed, actualLinesParsed );

			var expectedRegionsParsed = 1;
			var actualRegionsParsed = parser.RegionsParsed;
			Assert.AreEqual( expectedRegionsParsed, actualRegionsParsed );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingCloseBracket };
				yield return new object[] { TestData.RegionData_DanglingOpenBracket };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_Throws_FileRegionException()
		{
			_ = parser!.Parse( TestData.RegionData_Valid );
			_ = parser!.Parse( TestData.RegionData_Valid );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		public void Parse_Test_Throws_TokenException() => _ = parser!.Parse( TestData.RegionData_UnknownToken );
	}
}
