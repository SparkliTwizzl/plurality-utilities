using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.TestShared.Utilities;
using System.Data;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class MetadataRegionParserTests
	{
		public struct TestData
		{
			public static string[] RegionData_BlankVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				$"\t{CommonSyntax.MinimumVersionToken} {AppVersion.Current}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {AppVersion.Current}",
			};
			public static string[] RegionData_TooManyMinimumVersionTokens => new[]
			{
				$"{CommonSyntax.OpenBracketToken} {CommonSyntax.LineCommentToken} inline comment",
				$"\t{CommonSyntax.MinimumVersionToken} {AppVersion.Current}",
				$"\t{CommonSyntax.MinimumVersionToken} {AppVersion.Current}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnspecifiedPatchVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnspecifiedPatch}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnspecifiedPatchAndPreviewVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnspecifiedPatchAndPreview}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnsupportedMajorVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnsupportedMajor}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnsupportedMinorVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnsupportedMinor}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnsupportedPatchVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnsupportedPatch}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnsupportedPreviewVersion => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{CommonSyntax.MinimumVersionToken} {VersionNumber_UnsupportedPreview}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid => new[]
			{
				$"{CommonSyntax.OpenBracketToken} {CommonSyntax.LineCommentToken} inline comment",
				$"\t{CommonSyntax.MinimumVersionToken} {AppVersion.Current}",
				CommonSyntax.CloseBracketToken,
			};
			public static string VersionNumber_UnspecifiedPatch => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Patch}";
			public static string VersionNumber_UnspecifiedPatchAndPreview => $"{AppVersion.Major}.{AppVersion.Minor}";
			public static string VersionNumber_UnsupportedMajor => $"void.{AppVersion.Minor}.{AppVersion.Patch}{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedMinor => $"{AppVersion.Major}.void.{AppVersion.Patch}{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedPatch => $"{AppVersion.Major}.{AppVersion.Minor}.void{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedPreview => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Patch}.void";
		}


		public MetadataRegionParser Parser { get; set; } = new();


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			Parser = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expected = MetadataRegionParser.RegionIsValidMessage;
			var actual = Parser.Parse( regionData );
			Assert.AreEqual( expected, actual );
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
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketMismatchException( string[] regionData ) => _ = Parser.Parse( regionData );

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
			_ = Parser.Parse( TestData.RegionData_Valid );
			_ = Parser.Parse( TestData.RegionData_Valid );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = Parser.Parse( regionData );

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
		public void Parse_Test_Throws_VersionNotFoundException( string[] regionData ) => _ = Parser.Parse( regionData );

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
