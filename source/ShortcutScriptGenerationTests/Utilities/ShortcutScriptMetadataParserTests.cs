using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptMetadataParserTests
	{
		public struct TestData
		{
			public static string IconPath => "path/to/icon.ico";
			public static ShortcutScriptMetadata MetadataWithOptionalData => new( IconPath );
			public static ShortcutScriptMetadata MetadataWithoutOptionalData => new();
			public static string[] RegionDataWithDanglingCloseBracket => new[]
			{
				"}",
			};
			public static string[] RegionDataWithDanglingOpenBracket => new[]
			{
				"{",
			};
			public static string[] RegionDataWithUnknownToken => new[]
			{
				"{",
				"	unknown: token",
				"}",
			};
			public static string[] ValidRegionDataWithOptionalTokens => new[]
			{
				"{",
				$"	default-icon: { IconPath }",
				"}",
			};
			public static string[] ValidRegionDataWithoutOptionalTokens => new[]
			{
				"{",
				"}",
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
			var actual = metadataParser!.ParseMetadataFromData(TestData.ValidRegionDataWithOptionalTokens, ref i);
			Log.WriteLine();
			Log.Info( $"expected: default-icon={ expected.DefaultIconPath }" );
			Log.Info( $"actual: default-icon={ actual.DefaultIconPath }" );
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseMetadataFromDataTest_Success_NoOptionalTokens()
		{
			var expected = TestData.MetadataWithoutOptionalData;
			var actual = metadataParser!.ParseMetadataFromData(TestData.ValidRegionDataWithoutOptionalTokens, ref i);
			Log.WriteLine();
			Log.Info( $"expected: default-icon={ expected.DefaultIconPath }" );
			Log.Info( $"actual: default-icon={ actual.DefaultIconPath }" );
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof( UnknownTokenException ))]
		public void ParseMetadataFromDataTest_ThrowsUnknownTokenException()
		{
			_ = metadataParser!.ParseMetadataFromData(TestData.RegionDataWithUnknownToken, ref i);
		}

		[TestMethod]
		[ExpectedException(typeof( BracketMismatchException ))]
		[DynamicData(nameof(ParseMetadataFromDataTest_ThrowsBracketMismatchException_Data), DynamicDataSourceType.Property)]
		public void ParseMetadataFromDataTest_ThrowsBracketMismatchException(string[] regionData)
		{
			_ = metadataParser!.ParseMetadataFromData(regionData, ref i);
		}

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
