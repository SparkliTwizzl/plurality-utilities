using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.TestShared.Utilities;


namespace Petrichor.Common.Utilities.Tests
{
	[ TestClass ]
	public class StringExtensionsTests
	{
		public struct TestData
		{
			public static string[] ExtensionlessFileNames => new[]
			{
				"z:\\folder\\file",
				@"z:\folder\file",
				"z:/folder/file",
				"z:\\folder/file",
				@"z:\folder/file",
				"z:\\folder\\",
				@"z:\folder\",
				"z:/folder/",
				"z:\\folder/",
				@"z:\folder/",
				"file",
				"text",
				"",
				string.Empty,
			};
			public static string[] FileNames => new[]
			{
				"file.ext",
				"file.ext",
				"file.ext",
				"file.ext",
				"file.ext",
				"",
				"",
				"",
				"",
				"",
				"file.ext",
				"text",
				"",
				string.Empty,
			};
			public static string[] FilePathDirectories => new[]
			{
				"z:\\folder\\",
				@"z:\folder\",
				"z:/folder/",
				"z:\\folder/",
				@"z:\folder/",
				"z:\\folder\\",
				@"z:\folder\",
				"z:/folder/",
				"z:\\folder/",
				@"z:\folder/",
				string.Empty,
				string.Empty,
				"",
				string.Empty,
			};
			public static string[] FilePaths => new[]
			{
				"z:\\folder\\file.ext",
				@"z:\folder\file.ext",
				"z:/folder/file.ext",
				"z:\\folder/file.ext",
				@"z:\folder/file.ext",
				"z:\\folder\\",
				@"z:\folder\",
				"z:/folder/",
				"z:\\folder/",
				@"z:\folder/",
				"file.ext",
				"text",
				"",
				string.Empty,
			};
		}


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void GetDirectoryTest_AllCases()
		{
			var expected = TestData.FilePathDirectories;
			var outputData = new List< string >();
			foreach ( var input in TestData.FilePaths )
			{
				var result = input.GetDirectory();
				outputData.Add( result );
				Log.Info( result );
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		public void GetFileNameTest_AllCases()
		{
			var expected = TestData.FileNames;
			var outputData = new List<string>();
			foreach ( var input in TestData.FilePaths )
			{
				var result = input.GetFileName();
				outputData.Add( result );
				Log.Info( result );
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		public void RemoveFileExtensionTest_AllCases()
		{
			var expected = TestData.ExtensionlessFileNames;
			var outputData = new List< string >();
			foreach ( var input in TestData.FilePaths )
			{
				var result = input.RemoveFileExtension();
				outputData.Add( result );
				Log.Info( result );
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}