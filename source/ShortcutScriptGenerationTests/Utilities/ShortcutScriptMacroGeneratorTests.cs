using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptMacroGeneratorTests
	{
		public struct TestData
		{
			public static ShortcutScriptEntry[] Entries => new[]
			{
				new ShortcutScriptEntry( new List<ShortcutScriptIdentity>(){ new( "name", "tag" ) }, "pronoun", "decoration" ),
			};
			public static ShortcutScriptInput Input => new( ModuleOptions, Entries, Templates );
			public static string[] Macros => new[]
			{
				"::@tag:: name",
				"::@$&tag:: name pronoun decoration",
			};
			public static ShortcutScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFileName, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static string[] Templates => new[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public ShortcutScriptMacroGenerator? macroGenerator;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			macroGenerator = new ShortcutScriptMacroGenerator();
		}


		[TestMethod]
		public void Generate_Test_Success()
		{
			var expected = TestData.Macros;
			var actual = macroGenerator!.Generate( TestData.Input ).ToArray();

			Log.Info( "expected:" );
			foreach ( var line in expected )
			{
				Log.Info( $"[{line}]" );
			}
			Log.WriteLine();
			Log.Info( "actual:" );
			foreach ( var line in actual )
			{
				Log.Info( $"[{line}]" );
			}

			CollectionAssert.AreEqual( expected, actual );
		}
	}
}
