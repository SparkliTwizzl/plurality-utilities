using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;
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
			public static string EntryColorToken => $"{ShortcutScriptGenerationSyntax.EntryColorToken} {EntryColorValue}";
			public static string EntryDecorationValue => "DECORATION";
			public static string EntryDecorationToken => $"{ShortcutScriptGenerationSyntax.EntryDecorationToken} {EntryDecorationValue}";
			public static string EntryIDValue => "ID";
			public static string EntryIDToken => $"{ShortcutScriptGenerationSyntax.EntryIDToken} {EntryIDValue}";
			public static string EntryLastNameValue => "LAST_NAME";
			public static string EntryLastNameToken => $"{ShortcutScriptGenerationSyntax.EntryLastNameToken} {EntryLastNameTokenValue}";
			public static string EntryLastNameTokenValue => $"{EntryLastNameValue} @{EntryLastTagValue}";
			public static string EntryLastTagValue => "LAST_TAG";
			public static string EntryLastTagToken => $"{ShortcutScriptGenerationSyntax.EntryLastTagToken} {EntryLastTagValue}";
			public static string EntryNameValue => "NAME";
			public static string EntryNameToken => $"{ShortcutScriptGenerationSyntax.EntryNameToken} {EntryNameTokenValue}";
			public static string EntryNameTokenValue => $"{EntryNameValue} @{EntryTagValue}";
			public static string EntryPronounValue => "PRONOUN";
			public static string EntryPronounToken => $"{ShortcutScriptGenerationSyntax.EntryPronounToken} {EntryPronounValue}";
			public static string EntryTagValue => "TAG";
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
			};
			public static string[] RegionData_UnknownToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\tunknown{ CommonSyntax.TokenValueDivider } token",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid_AllOptionalTokens => new[]
			{
				$"{ CommonSyntax.OpenBracketToken } { CommonSyntax.LineCommentToken } inline comment",
				string.Empty,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				$"\t{ EntryIDToken }",
				$"\t{ EntryNameToken }",
				$"\t{ EntryNameToken }",
				$"\t{ EntryLastNameToken }",
				$"\t{ EntryPronounToken }",
				$"\t{ EntryColorToken }",
				$"\t{ EntryDecorationToken }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ EntryIDToken }",
				$"\t{ EntryNameToken }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoIDToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoNameTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryIDToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoTagInNameToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameValue }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NameTokenContainsTagStartSymbol => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameValue }@text @{ EntryTagValue }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyColorTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryColorToken}",
				$"\t{EntryColorToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyDecorationTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryDecorationToken}",
				$"\t{EntryDecorationToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyIDTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryIDToken}",
				$"\t{EntryIDToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyLastNameTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryLastNameToken}",
				$"\t{EntryLastNameToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyPronounTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryPronounToken}",
				$"\t{EntryPronounToken}",
				CommonSyntax.CloseBracketToken,
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
