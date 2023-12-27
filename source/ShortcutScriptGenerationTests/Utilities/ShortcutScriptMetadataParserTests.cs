using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptMetadataParserTests
	{
		public struct TestData
		{
			public static string DefaultIconPath => "path/to/defaulticon.ico";
			public static string SuspendIconPath => "path/to/suspendicon.ico";
			public static ShortcutScriptMetadata MetadataWithOptionalData => new( DefaultIconPath, SuspendIconPath, ReloadShortcut, SuspendShortcut );
			public static ShortcutScriptMetadata MetadataWithoutOptionalData => new();
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
				"\tunknown: token",
				CommonSyntax.CloseBracketToken,
			};
			public static string ReloadShortcut => "reloadshortcut";
			public static string SuspendShortcut => "suspendshortcut";
			public static string[] ValidRegionDataWithOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken }: comment",
				string.Empty,
				$"\t{ ShortcutScriptGenerationSyntax.DefaultIconFilePathToken }: { DefaultIconPath }",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendIconFilePathToken }: { SuspendIconPath }",
				$"\t{ ShortcutScriptGenerationSyntax.ReloadShortcutToken }: { ReloadShortcut }",
				$"\t{ ShortcutScriptGenerationSyntax.SuspendShortcutToken }: { SuspendShortcut }",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] ValidRegionDataWithoutOptionalTokens => new[]
			{
				CommonSyntax.OpenBracketToken,
				CommonSyntax.CloseBracketToken,
			};
		}


		public int i;
		public ShortcutScriptMetadataParser? metadataParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 0;
			metadataParser = new();
		}


		[TestMethod]
		public void ParseMetadataFromDataTest_Success_AllOptionalTokens()
		{
			var expected = TestData.MetadataWithOptionalData;
			var actual = metadataParser!.ParseMetadataFromData( TestData.ValidRegionDataWithOptionalTokens, ref i );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		public void ParseMetadataFromDataTest_Success_NoOptionalTokens()
		{
			var expected = TestData.MetadataWithoutOptionalData;
			var actual = metadataParser!.ParseMetadataFromData( TestData.ValidRegionDataWithoutOptionalTokens, ref i );
			Assert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( UnknownTokenException ) )]
		public void ParseMetadataFromDataTest_ThrowsUnknownTokenException()
			=> _ = metadataParser!.ParseMetadataFromData( TestData.RegionDataWithUnknownToken, ref i );

		[TestMethod]
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( ParseMetadataFromDataTest_ThrowsBracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void ParseMetadataFromDataTest_ThrowsBracketMismatchException( string[] regionData )
			=> _ = metadataParser!.ParseMetadataFromData( regionData, ref i );

		public static IEnumerable<object[]> ParseMetadataFromDataTest_ThrowsBracketMismatchException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionDataWithDanglingCloseBracket };
				yield return new object[] { TestData.RegionDataWithDanglingOpenBracket };
			}
		}
	}
}
