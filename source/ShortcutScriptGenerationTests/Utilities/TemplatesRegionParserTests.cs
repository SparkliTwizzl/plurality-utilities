using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Syntax;
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
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingEscapeCharacter => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"{ Tokens.Template } \t{ TemplateFindStrings.Tag } :: { Common.Syntax.OperatorChars.Escape }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingFindStringCloseChar => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Tokens.Template } { Common.Syntax.OperatorChars.TokenNameClose }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingFindStringOpenChar => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Tokens.Template } { Common.Syntax.OperatorChars.TokenNameOpen }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
			};
			public static string[] RegionData_MismatchedFindStringCloseChar => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Tokens.Template } find{ Common.Syntax.OperatorChars.TokenNameClose } string",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_MismatchedFindStringOpenChar => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Tokens.Template } { Common.Syntax.OperatorChars.TokenNameOpen }find string",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_UnknownFindStringValue => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Tokens.Template } { Common.Syntax.OperatorChars.TokenNameOpen }unknown{ Common.Syntax.OperatorChars.TokenNameClose }",
				Common.Syntax.Tokens.RegionClose,
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
				string.Empty,
				$"\t{ Common.Syntax.TokenNames.LineComment } line comment",
				string.Empty,
				$"\t{ Tokens.Template } { RawFindString } :: { RawReplaceString } { Common.Syntax.TokenNames.LineComment } inline comment",
				string.Empty,
				$"\t{ Tokens.Template } { RawFindString } :: { RawReplaceString }",
				string.Empty,
				Common.Syntax.Tokens.RegionClose,
			};
			public static string Template => $"::{ ParsedFindString }::{ ParsedReplaceString }";
			public static string ParsedFindString
				=> $"{ Common.Syntax.OperatorChars.EscapeStandin }{ Common.Syntax.OperatorChars.TokenNameOpenStandin }{ TemplateFindStrings.Tag }{ TemplateFindStrings.LastTag }";
			public static string RawFindString
				=> $"{ Common.Syntax.OperatorChars.Escape }{ Common.Syntax.OperatorChars.Escape }{ Common.Syntax.OperatorChars.Escape }{ Common.Syntax.OperatorChars.TokenNameOpen }{ TemplateFindStrings.Tag }{ TemplateFindStrings.LastTag }";
			public static string ParsedReplaceString
				=> $"{ Common.Syntax.OperatorChars.TokenNameOpenStandin }{ TemplateFindStrings.ID }{ Common.Syntax.OperatorChars.TokenNameCloseStandin } { TemplateFindStrings.Name } { TemplateFindStrings.LastName } { TemplateFindStrings.Pronoun } { TemplateFindStrings.Color } { TemplateFindStrings.Decoration } `";
			public static string RawReplaceString
				=> $"{ Common.Syntax.OperatorChars.Escape }{ Common.Syntax.OperatorChars.TokenNameOpen }{ TemplateFindStrings.ID }{ Common.Syntax.OperatorChars.Escape }{ Common.Syntax.OperatorChars.TokenNameClose } { TemplateFindStrings.Name } { TemplateFindStrings.LastName } { TemplateFindStrings.Pronoun } { TemplateFindStrings.Color } { TemplateFindStrings.Decoration } `";
			public static string[] Templates => new[]
			{
				Template,
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
		[ExpectedException( typeof( EscapeCharacterException ) )]
		public void Parse_Test_Throws_EscapeCharacterException() => _ = parser!.Parse( TestData.RegionData_DanglingEscapeCharacter );

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_Throws_FileRegionException()
		{
			_ = parser!.Parse( TestData.RegionData_Valid );
			_ = parser!.Parse( TestData.RegionData_Valid );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingFindStringCloseChar };
				yield return new object[] { TestData.RegionData_DanglingFindStringOpenChar };
				yield return new object[] { TestData.RegionData_MismatchedFindStringCloseChar };
				yield return new object[] { TestData.RegionData_MismatchedFindStringOpenChar };
				yield return new object[] { TestData.RegionData_UnknownFindStringValue };
				yield return new object[] { TestData.RegionData_UnknownToken };
			}
		}
	}
}
