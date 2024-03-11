using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class EntryRegionParserTests
	{
		public struct TestData
		{
			public static ScriptEntry Entry_AllOptionalData => new(
					EntryIDValue,
					new List<ScriptIdentity>
					{
						new(EntryNameValue, EntryTagValue),
						new(EntryNameValue, EntryTagValue),
					},
					new ScriptIdentity( EntryLastNameValue, EntryLastTagValue ),
					EntryPronounValue,
					EntryColorValue,
					EntryDecorationValue
				);
			public static ScriptEntry Entry_NoOptionalData => new( EntryIDValue, new List<ScriptIdentity> { new( EntryNameValue, EntryTagValue ) } );
			public static string EntryColorValue => "COLOR";
			public static string EntryColorToken => $"{Syntax.Tokens.EntryColor} {EntryColorValue}";
			public static string EntryDecorationValue => "DECORATION";
			public static string EntryDecorationToken => $"{Syntax.Tokens.EntryDecoration} {EntryDecorationValue}";
			public static string EntryIDValue => "ID";
			public static string EntryIDToken => $"{Syntax.Tokens.EntryID} {EntryIDValue}";
			public static string EntryLastNameValue => "LAST_NAME";
			public static string EntryLastNameToken => $"{Syntax.Tokens.EntryLastName} {EntryLastNameTokenValue}";
			public static string EntryLastNameTokenValue => $"{EntryLastNameValue} @{EntryLastTagValue}";
			public static string EntryLastTagValue => "LAST_TAG";
			public static string EntryLastTagToken => $"{Syntax.Tokens.EntryLastTag} {EntryLastTagValue}";
			public static string EntryNameValue => "NAME";
			public static string EntryNameToken => $"{Syntax.Tokens.EntryName} {EntryNameTokenValue}";
			public static string EntryNameTokenValue => $"{EntryNameValue} @{EntryTagValue}";
			public static string EntryPronounValue => "PRONOUN";
			public static string EntryPronounToken => $"{Syntax.Tokens.EntryPronoun} {EntryPronounValue}";
			public static string EntryTagValue => "TAG";
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
			public static string[] RegionData_Valid_AllOptionalTokens => new[]
			{
				$"{ Common.Syntax.Tokens.RegionOpen } { Common.Syntax.Tokens.LineComment } inline comment",
				string.Empty,
				$"\t{ Common.Syntax.Tokens.LineComment } line comment",
				$"\t{ EntryIDToken }",
				$"\t{ EntryNameToken }",
				$"\t{ EntryNameToken }",
				$"\t{ EntryLastNameToken }",
				$"\t{ EntryPronounToken }",
				$"\t{ EntryColorToken }",
				$"\t{ EntryDecorationToken }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ EntryIDToken }",
				$"\t{ EntryNameToken }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_NoIDToken => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_NoNameTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryIDToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_NoTagInNameToken => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Syntax.Tokens.EntryName } { EntryNameValue }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_NameTokenContainsTagStartSymbol => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Syntax.Tokens.EntryName } { EntryNameValue }@text @{ EntryTagValue }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyColorTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				$"\t{EntryColorToken}",
				$"\t{EntryColorToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyDecorationTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				$"\t{EntryDecorationToken}",
				$"\t{EntryDecorationToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyIDTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				$"\t{EntryIDToken}",
				$"\t{EntryIDToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyLastNameTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				$"\t{EntryLastNameToken}",
				$"\t{EntryLastNameToken}",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyPronounTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{EntryNameToken}",
				$"\t{EntryPronounToken}",
				$"\t{EntryPronounToken}",
				Common.Syntax.Tokens.RegionClose,
			};
		}


		EntryRegionParser? parser;

		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData, ScriptEntry expectedResult )
		{
			var actualResult = parser!.Parse( regionData );
			Assert.AreEqual( expectedResult, actualResult );

			var expectedHasParsedMaxAllowedRegions = false;
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
				yield return new object[] { TestData.RegionData_Valid_AllOptionalTokens, TestData.Entry_AllOptionalData };
				yield return new object[] { TestData.RegionData_Valid_NoOptionalTokens, TestData.Entry_NoOptionalData };
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
		[DynamicData( nameof( Parse_Test_Throws_FileRegionException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_FileRegionException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_FileRegionException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_NoIDToken };
				yield return new object[] { TestData.RegionData_NoNameTokens };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_TooManyDecorationTokens };
				yield return new object[] { TestData.RegionData_NoTagInNameToken };
				yield return new object[] { TestData.RegionData_TooManyPronounTokens };
				yield return new object[] { TestData.RegionData_UnknownToken };
			}
		}
	}
}
