using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptMacroParserTests
	{
		public struct TestData
		{
			public static ShortcutScriptEntry[] Entries => new[]
			{
				new ShortcutScriptEntry( new List<ShortcutScriptIdentity>(){ new( "name", "tag" ) }, "pronoun", "decoration" ),
			};
			public static ShortcutScriptInput Input => new(Metadata, Entries, Templates);
			public static string[] Macros => new[]
			{
				"::@tag:: name",
				"::@$&tag:: name pronoun decoration",
			};
			public static ShortcutScriptMetadata Metadata => new(TestAssets.DefaultIconFileName);
			public static string[] Templates => new[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public ShortcutScriptMacroParser? macroParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			macroParser = new ShortcutScriptMacroParser();
		}


		[TestMethod]
		public void GenerateMacrosFromTemplatesTest_Success()
		{
			var expected = TestData.Macros;
			var actual = macroParser!.GenerateMacrosFromInput(TestData.Input).ToArray();

			Log.Info("expected:");
			foreach (var line in expected)
			{
				Log.Info($"[{line}]");
			}
			Log.WriteLine();
			Log.Info("actual:");
			foreach (var line in actual)
			{
				Log.Info($"[{line}]");
			}

			CollectionAssert.AreEqual(expected, actual);
		}
	}
}
