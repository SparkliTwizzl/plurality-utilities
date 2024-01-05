using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class TemplatesRegionParserTests
	{
		public struct TestData
		{
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
			};
			public static string[] RegionData_TrailingExcapeCharacter => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"{ ShortcutScriptGenerationSyntax.TemplateToken } \t::[tag]:: [name]\\",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnknownToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\tunknown{ CommonSyntax.TokenValueDivider } token",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				string.Empty,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { Template_Valid } { CommonSyntax.LineCommentToken } inline comment",
				CommonSyntax.CloseBracketToken,
			};
			public static string Template_Valid => "::@[tag]:: \\[[name]\\] [pronoun] [decoration] `";
			public static string[] Templates => new[]
			{
				Template_Valid,
			};
		}


		public TemplatesRegionParser? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData, string[] expectedResult )
		{
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

			var expectedTemplatesParsed = expectedResult.Length;
			var actualTemplatesParsed = parser.TemplatesParsed;
			Assert.AreEqual( expectedTemplatesParsed, actualTemplatesParsed );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid, TestData.Templates };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketMismatchException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketMismatchException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingCloseBracket };
				yield return new object[] { TestData.RegionData_DanglingOpenBracket };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( EscapeCharacterMismatchException ) )]
		public void Parse_Test_Throws_EscapeCharacterMismatchException() => _ = parser!.Parse( TestData.RegionData_TrailingExcapeCharacter );

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
