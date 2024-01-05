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
			public static string[] RegionData_DanglingExcapeCharacter => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"{ ShortcutScriptGenerationSyntax.TemplateToken } \t::[tag]:: [name]\\",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingFindStringCloseChar => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingFindStringOpenChar => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
			};
			public static string[] RegionData_MismatchedFindStringCloseChar => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } find{ ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar } string",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_MismatchedFindStringOpenChar => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar }find string",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnknownFindStringValue => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar }unknown{ ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar }",
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
				$"\t{ ShortcutScriptGenerationSyntax.TemplateToken } { Template } { CommonSyntax.LineCommentToken } inline comment",
				CommonSyntax.CloseBracketToken,
			};
			public static string Template => $"::@{ShortcutScriptGenerationSyntax.TemplateFindTagString}:: \\{ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar}{ShortcutScriptGenerationSyntax.TemplateFindNameString}\\{ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar} {ShortcutScriptGenerationSyntax.TemplateFindPronounString} {ShortcutScriptGenerationSyntax.TemplateFindDecorationString} `";
			public static string[] Templates => new[]
			{
				Template,
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
		[ExpectedException( typeof( BracketException ) )]
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
		[ExpectedException( typeof( EscapeCharacterException ) )]
		public void Parse_Test_Throws_EscapeCharacterMismatchException() => _ = parser!.Parse( TestData.RegionData_DanglingExcapeCharacter );

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
