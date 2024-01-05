using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ModuleOptionsRegionParserTests
	{
		public struct TestData
		{
			public static string DefaultIconPath => "path/to/defaulticon.ico";
			public static string DefaultIconPathWithQuotes => $"\"{DefaultIconPath}\"";
			public static ScriptModuleOptions ModuleOptions_Valid_NoOptionalData => new();
			public static ScriptModuleOptions ModuleOptions_Valid_AllOptionalData => new( DefaultIconPathWithQuotes, SuspendIconPathWithQuotes, ReloadShortcut, SuspendShortcut );
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
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				string.Empty,
				$"\t{ ShortcutScriptGenerationSyntax.DefaultIconFilePathToken } \"{ DefaultIconPath }\" { CommonSyntax.LineCommentToken } inline comment",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendIconFilePathToken } \"{ SuspendIconPath }\"",
				$"\t{ ShortcutScriptGenerationSyntax.ReloadShortcutToken } { ReloadShortcut }",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendShortcutToken } { SuspendShortcut }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				CommonSyntax.CloseBracketToken,
			};
			public static string ReloadShortcut => "reloadshortcut";
			public static string SuspendIconPath => "path/to/suspendicon.ico";
			public static string SuspendIconPathWithQuotes => $"\"{SuspendIconPath}\"";
			public static string SuspendShortcut => "suspendshortcut";
		}


		public ModuleOptionsRegionParser? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData, ScriptModuleOptions expectedResult )
		{
			var actualResult = parser!.Parse( regionData );
			Assert.AreEqual( expectedResult, actualResult );
			
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
				yield return new object[] { TestData.RegionData_Valid_AllOptionalTokens, TestData.ModuleOptions_Valid_AllOptionalData };
				yield return new object[] { TestData.RegionData_Valid_NoOptionalTokens, TestData.ModuleOptions_Valid_NoOptionalData };
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
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		public void Parse_Test_Throws_TokenException() => _ = parser!.Parse( TestData.RegionData_UnknownToken );
	}
}
