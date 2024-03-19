using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.TestShared.Utilities;
using System.Data;


namespace Petrichor.Common.Info.Tests
{
	[TestClass]
	public class AppVersionTests
	{
		public struct TestData
		{
			public static string VersionNumber_Blank => string.Empty;
			public static string VersionNumber_FullCurrent => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Patch}{AppVersion.Preview}";
			public static string VersionNumber_UnspecifiedPatchAndPreview => $"{AppVersion.Major}.{AppVersion.Minor}";
			public static string VersionNumber_UnspecifiedPreview => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Patch}";
			public static string VersionNumber_UnsupportedMajor => $"void.{AppVersion.Minor}.{AppVersion.Patch}{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedMinor => $"{AppVersion.Major}.void.{AppVersion.Patch}{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedPatch => $"{AppVersion.Major}.{AppVersion.Minor}.void{AppVersion.Preview}";
			public static string VersionNumber_UnsupportedPreview => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Patch}.void";
		}


		[TestInitialize]
		public void Setup() => TestUtilities.InitializeLoggingForTests();


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void RejectUnsupportedVersions_Test_Success( string version ) => AppVersion.RejectUnsupportedVersions( version );

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.VersionNumber_FullCurrent };
				yield return new object[] { TestData.VersionNumber_UnspecifiedPatchAndPreview };
				yield return new object[] { TestData.VersionNumber_UnspecifiedPreview };
			}
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Throws_VersionNotFoundException_Data ), DynamicDataSourceType.Property )]
		[ExpectedException( typeof( VersionNotFoundException ) )]
		public void RejectUnsupportedVersions_Test_Throws_VersionNotFoundException( string version ) => AppVersion.RejectUnsupportedVersions( version );

		public static IEnumerable<object[]> Parse_Test_Throws_VersionNotFoundException_Data
		{
			get
			{
				yield return new object[] { TestData.VersionNumber_Blank };
				yield return new object[] { TestData.VersionNumber_UnsupportedMajor };
				yield return new object[] { TestData.VersionNumber_UnsupportedMinor };
				yield return new object[] { TestData.VersionNumber_UnsupportedPatch };
				yield return new object[] { TestData.VersionNumber_UnsupportedPreview };
			}
		}
	}
}
