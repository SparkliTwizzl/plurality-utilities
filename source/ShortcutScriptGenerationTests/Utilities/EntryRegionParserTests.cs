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
					new List<ShortcutScriptIdentity>
					{
						new(EntryName, EntryTag),
						new(EntryName, EntryTag),
					},
					EntryPronoun,
					EntryDecoration
				);
			public static ScriptEntry Entry_NoOptionalData => new(
					new List<ShortcutScriptIdentity>
					{
						new(EntryName, EntryTag),
					},
					EntryPronoun,
					EntryDecoration
				);
			public static string EntryDecoration => "decoration";
			public static string EntryName => "name";
			public static string EntryNameTokenValue => $"{EntryName} @{EntryTag}";
			public static string EntryPronoun => "pronoun";
			public static string EntryTag => "tag";
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
				$"{ CommonSyntax.OpenBracketToken }",
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue } { CommonSyntax.LineCommentToken } inline comment",
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue }",
				$"\t{EntryPronoun}",
				$"\t{EntryDecoration}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoTagInNameToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryName }",
				$"\t{EntryPronoun}",
				$"\t{EntryDecoration}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyPronounTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue }",
				$"\t{EntryPronoun}",
				$"\t{EntryPronoun}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyDecorationTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue }",
				$"\t{EntryDecoration}",
				$"\t{EntryDecoration}",
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
		public void Parse_Test_Success_AllOptionalTokens()
		{
			var expected = TestData.Entry_AllOptionalData;
			var actual = parser!.Parse( TestData.RegionData_Valid_AllOptionalTokens );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		public void Parse_Test_Success_NoOptionalTokens()
		{
			var expected = TestData.Entry_NoOptionalData;
			var actual = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
			Assert.AreEqual( expected, actual );
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
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_Throws_FileRegionException()
		{
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
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
