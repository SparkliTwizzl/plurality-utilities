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
					new List<ScriptIdentity>
					{
						new(EntryNameValue, EntryTagValue),
						new(EntryNameValue, EntryTagValue),
					},
					EntryPronounValue,
					EntryDecorationValue
				);
			public static ScriptEntry Entry_NoOptionalData => new(
					new List<ScriptIdentity>
					{
						new( EntryNameValue, EntryTagValue ),
					},
					string.Empty,
					string.Empty
				);
			public static string EntryDecorationValue => "decoration";
			public static string EntryDecorationToken => $"{ShortcutScriptGenerationSyntax.EntryDecorationToken} {EntryDecorationValue}";
			public static string EntryNameValue => "name";
			public static string EntryNameToken => $"{ShortcutScriptGenerationSyntax.EntryNameToken} {EntryNameTokenValue}";
			public static string EntryNameTokenValue => $"{EntryNameValue} @{EntryTagValue}";
			public static string EntryPronounValue => "pronoun";
			public static string EntryPronounToken => $"{ShortcutScriptGenerationSyntax.EntryPronounToken} {EntryPronounValue}";
			public static string EntryTagValue => "tag";
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
				$"\t{ EntryNameToken }",
				$"\t{ EntryNameToken }",
				$"\t{ EntryPronounToken }",
				$"\t{ EntryDecorationToken }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ EntryNameToken }",
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
			public static string[] RegionData_TooManyPronounTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{EntryNameToken}",
				$"\t{EntryPronounToken}",
				$"\t{EntryPronounToken}",
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
		public void Parse_Test_Success( string[] regionData, ScriptEntry expected )
		{
			var actual = parser!.Parse( regionData );
			Assert.AreEqual( expected, actual );
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
