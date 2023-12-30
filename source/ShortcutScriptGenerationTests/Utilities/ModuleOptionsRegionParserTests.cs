using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
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
			public static string SuspendIconPath => "path/to/suspendicon.ico";
			public static ShortcutScriptModuleOptions ModuleOptionsWithOptionalData => new( DefaultIconPath, SuspendIconPath, ReloadShortcut, SuspendShortcut );
			public static ShortcutScriptModuleOptions ModuleOptionsWithoutOptionalData => new();
			public static string[] RegionDataWithDanglingCloseBracket => new[]
			{
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionDataWithDanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
			};
			public static string[] RegionDataWithUnknownToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\tunknown{ CommonSyntax.TokenValueDivider } token",
				CommonSyntax.CloseBracketToken,
			};
			public static string ReloadShortcut => "reloadshortcut";
			public static string SuspendShortcut => "suspendshortcut";
			public static string[] ValidRegionDataWithOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				string.Empty,
				$"\t{ ShortcutScriptGenerationSyntax.DefaultIconFilePathToken } { DefaultIconPath } { CommonSyntax.LineCommentToken } inline comment",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendIconFilePathToken } { SuspendIconPath }",
				$"\t{ ShortcutScriptGenerationSyntax.ReloadShortcutToken } { ReloadShortcut }",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendShortcutToken } { SuspendShortcut }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] ValidRegionDataWithoutOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				CommonSyntax.CloseBracketToken,
			};
		}


		public int i;
		public ModuleOptionsRegionParser? moduleOptionsRegionParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 0;
			moduleOptionsRegionParser = new();
		}


		[TestMethod]
		public void ParseModuleOptionsFromData_Test_Success_AllOptionalTokens()
		{
			var expected = TestData.ModuleOptionsWithOptionalData;
			var actual = moduleOptionsRegionParser!.ParseModuleOptionsFromData( TestData.ValidRegionDataWithOptionalTokens, ref i );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		public void ParseModuleOptionsFromData_Test_Success_NoOptionalTokens()
		{
			var expected = TestData.ModuleOptionsWithoutOptionalData;
			var actual = moduleOptionsRegionParser!.ParseModuleOptionsFromData( TestData.ValidRegionDataWithoutOptionalTokens, ref i );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( ParseModuleOptionsFromData_Test_ThrowsBracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void ParseModuleOptionsFromData_Test_ThrowsBracketMismatchException( string[] regionData )
			=> _ = moduleOptionsRegionParser!.ParseModuleOptionsFromData( regionData, ref i );

		public static IEnumerable<object[]> ParseModuleOptionsFromData_Test_ThrowsBracketMismatchException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionDataWithDanglingCloseBracket };
				yield return new object[] { TestData.RegionDataWithDanglingOpenBracket };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( UnknownTokenException ) )]
		public void ParseModuleOptionsFromData_Test_ThrowsUnknownTokenException()
			=> _ = moduleOptionsRegionParser!.ParseModuleOptionsFromData( TestData.RegionDataWithUnknownToken, ref i );
	}
}
