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
		public void Parse_Test_Success()
		{
			var expected = MetadataRegionParser.RegionIsValidMessage;
			var actual = Parser.Parse( TestData.RegionData_Valid );
			Assert.AreEqual( expected, actual );
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
		[DynamicData( nameof(Parse_Test_Throws_TokenException_Data), DynamicDataSourceType.Property )]
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
		[DynamicData( nameof(Parse_Test_Throws_VersionNotFoundException_Data), DynamicDataSourceType.Property )]
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
	}
}
