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
			public static ScriptModuleOptions ModuleOptions_Valid_OptionalData => new( DefaultIconPathWithQuotes, SuspendIconPathWithQuotes, ReloadShortcut, SuspendShortcut );
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
			public static string[] RegionData_Valid_OptionalTokens => new[]
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
		public void Parse_Test_Success_AllOptionalTokens()
		{
			var expected = TestData.ModuleOptions_Valid_OptionalData;
			var actual = parser!.Parse( TestData.RegionData_Valid_OptionalTokens );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		public void Parse_Test_Success_NoOptionalTokens()
		{
			var expected = TestData.ModuleOptions_Valid_NoOptionalData;
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
		public void Parse_Test_Throws_TokenException() => _ = parser!.Parse( TestData.RegionData_UnknownToken );
	}
}
