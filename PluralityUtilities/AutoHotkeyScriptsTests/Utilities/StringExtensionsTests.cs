using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{
		public static readonly string[] inputData = new string[]
		{
			"z:\\folder\\file.ext",
			"z:/folder/file.ext",
			"z:\\folder/file.ext",
			"z:\\folder\\",
			"z:/folder/",
			"z:\\folder/",
			"file.ext",
			"text",
		};


		[TestMethod]
		public void GetDirectoryTest_MixedInputs()
		{
			var expected = new string[]
			{
				"z:\\folder\\",
				"z:/folder/",
				"z:\\folder/",
				"z:\\folder\\",
				"z:/folder/",
				"z:\\folder/",
				string.Empty,
				string.Empty,
			};
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				outputData.Add(input.GetDirectory());
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetFileNameTest_MixedInputs()
		{
			var expected = new string[]
			{
				"file.ext",
				"file.ext",
				"file.ext",
				"",
				"",
				"",
				"file.ext",
				"text",
			};
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				outputData.Add(input.GetFileName());
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveFileExtensionTest_MixedInputs()
		{
			var expected = new string[]
			{
				"z:\\folder\\file",
				"z:/folder/file",
				"z:\\folder/file",
				"z:\\folder\\",
				"z:/folder/",
				"z:\\folder/",
				"file",
				"text",
			};
			var outputData = new List<string>();
			foreach (var input in inputData)
			{
				outputData.Add(input.RemoveFileExtension());
			}
			var actual = outputData.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}