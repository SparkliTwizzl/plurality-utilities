using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Syntax;
using Petrichor.TestShared.Utilities;
using System.Data;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class MetadataRegionHandlerTests
	{
		public struct TestData
		{
			public static string[] RegionData_BlankVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				$"\t{ Tokens.MinimumVersion } { AppVersion.Current }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { AppVersion.Current }",
			};
			public static string[] RegionData_TooManyMinimumVersionTokens => new[]
			{
				$"{ Tokens.RegionOpen } { Tokens.LineComment } inline comment",
				$"\t{ Tokens.MinimumVersion } { AppVersion.Current }",
				$"\t{ Tokens.MinimumVersion } { AppVersion.Current }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnspecifiedPatchVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnspecifiedPatch }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnspecifiedPatchAndPreviewVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnspecifiedPatchAndPreview }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnsupportedMajorVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnsupportedMajor }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnsupportedMinorVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnsupportedMinor }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnsupportedPatchVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnsupportedPatch }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_UnsupportedPreviewVersion => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Tokens.MinimumVersion } { VersionNumber_UnsupportedPreview }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_Valid => new[]
			{
				$"{ Tokens.RegionOpen } { Tokens.LineComment } inline comment",
				$"\t{ Tokens.MinimumVersion } { AppVersion.Current }",
				Tokens.RegionClose,
			};
			public static string VersionNumber_UnspecifiedPatch => $"{ AppVersion.Major }.{ AppVersion.Minor }.{ AppVersion.Patch }";
			public static string VersionNumber_UnspecifiedPatchAndPreview => $"{ AppVersion.Major }.{ AppVersion.Minor }";
			public static string VersionNumber_UnsupportedMajor => $"void.{ AppVersion.Minor }.{ AppVersion.Patch }{ AppVersion.Preview }";
			public static string VersionNumber_UnsupportedMinor => $"{ AppVersion.Major }.void.{ AppVersion.Patch }{ AppVersion.Preview }";
			public static string VersionNumber_UnsupportedPatch => $"{ AppVersion.Major }.{ AppVersion.Minor }.void{ AppVersion.Preview }";
			public static string VersionNumber_UnsupportedPreview => $"{ AppVersion.Major }.{ AppVersion.Minor }.{ AppVersion.Patch }.void";
		}


		public MetadataRegionHandler? regionHandler;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			regionHandler = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expectedResult = new StringWrapper();
			var actualResult = regionHandler!.Parser.Parse( regionData );
			Assert.AreEqual( expectedResult, actualResult );

			var expectedHasParsedMaxAllowedRegions = true;
			var actualHasParsedMaxAllowedRegions = regionHandler.Parser.HasParsedMaxAllowedRegions;
			Assert.AreEqual( expectedHasParsedMaxAllowedRegions, actualHasParsedMaxAllowedRegions );

			var expectedLinesParsed = regionData.Length;
			var actualLinesParsed = regionHandler.Parser.LinesParsed;
			Assert.AreEqual( expectedLinesParsed, actualLinesParsed );

			var expectedRegionsParsed = 1;
			var actualRegionsParsed = regionHandler.Parser.RegionsParsed;
			Assert.AreEqual( expectedRegionsParsed, actualRegionsParsed );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid };
				yield return new object[] { TestData.RegionData_UnspecifiedPatchVersion };
				yield return new object[] { TestData.RegionData_UnspecifiedPatchAndPreviewVersion };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketException( string[] regionData ) => _ = regionHandler!.Parser.Parse( regionData );

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
			_ = regionHandler!.Parser.Parse( TestData.RegionData_Valid );
			_ = regionHandler!.Parser.Parse( TestData.RegionData_Valid );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = regionHandler!.Parser.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_BlankVersion };
				yield return new object[] { TestData.RegionData_TooManyMinimumVersionTokens };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( VersionNotFoundException ) )]
		[DynamicData( nameof( Parse_Test_Throws_VersionNotFoundException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_VersionNotFoundException( string[] regionData ) => _ = regionHandler!.Parser.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_VersionNotFoundException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_UnsupportedMajorVersion };
				yield return new object[] { TestData.RegionData_UnsupportedMinorVersion };
				yield return new object[] { TestData.RegionData_UnsupportedPatchVersion };
				yield return new object[] { TestData.RegionData_UnsupportedPreviewVersion };
			}
		}
	}
}
