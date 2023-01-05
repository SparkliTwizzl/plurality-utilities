using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon;


namespace PluralityUtilities.Common.Utilities.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{
		public static readonly string[] inputData = new string[]
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


		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
		}


		[TestMethod]
		public void GetDirectoryTest_AllCases()
		{
			var expected = new string[]
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
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				var result = input.GetDirectory();
				outputData.Add(result);
				Log.WriteLine(result);
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetFileNameTest_AllCases()
		{
			var expected = new string[]
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
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				var result = input.GetFileName();
				outputData.Add(result);
				Log.WriteLine(result);
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveFileExtensionTest_AllCases()
		{
			var expected = new string[]
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
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				var result = input.RemoveFileExtension();
				outputData.Add(result);
				Log.WriteLine(result);
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}