using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.AutoHotkeyScripts.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.TestShared.Utilities;


namespace Petrichor.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class AutoHotkeyScriptGeneratorTests
	{
		public static class TestData
		{
			public static readonly Entry[] Entries = new Entry[]
			{
				new Entry( new List<Identity>(){ new Identity( "name", "tag" ) }, "pronoun", "decoration" ),
			};
			public static readonly string[] Templates = new string[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
			public static readonly Input Input = new Input( Entries, Templates );
			public static readonly string[] Macros = new string[]
			{
				"::@tag:: name",
				"::@$&tag:: name pronoun decoration",
			};
			public static readonly string[] GeneratedOutputFileContents = new string[]
			{
				$"; Generated with { AppInfo.AppName } v{ AppInfo.CurrentVersion }",
				"",
				"",
				"#SingleInstance Force",
				"",
				"::@tag:: name",
				"::@$&tag:: name pronoun decoration",
			};
		}


		public AutoHotkeyScriptGenerator? ScriptGenerator;


		[TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			ScriptGenerator = new AutoHotkeyScriptGenerator();
		}


		[ TestMethod ]
		public void GenerateMacrosFromTemplatesTest_Success()
		{
			var expected = TestData.Macros;
			var actual = ScriptGenerator.GenerateMacrosFromInput( TestData.Input ).ToArray();

			Log.WriteLine( "expected:" );
			foreach ( var line in expected )
			{
				Log.WriteLine( $"[{ line }]" );
			}
			Log.WriteLine();
			Log.WriteLine( "actual:" );
			foreach ( var line in actual )
			{
				Log.WriteLine( $"[{ line }]" );
			}

			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		public void GenerateScriptTest_Success()
		{
			var outputFile = $@"{ TestDirectories.TestOutputDirectory }\{ nameof( AutoHotkeyScriptGenerator ) }_{ nameof( GenerateScriptTest_Success ) }.ahk";
			ScriptGenerator.GenerateScript( TestData.Macros, outputFile );

			var expected = TestData.GeneratedOutputFileContents;
			var actual = File.ReadAllLines( outputFile );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}