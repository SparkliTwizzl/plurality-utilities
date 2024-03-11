using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ModuleOptionsRegionParserTests
	{
		public struct TestData
		{
			public static string DefaultIconPath => "path/to/defaulticon.ico";
			public static string DefaultIconPathWithQuotes => $"\"{DefaultIconPath}\"";
			public static ScriptModuleOptions ModuleOptions_Valid_NoOptionalData => new();
			public static ScriptModuleOptions ModuleOptions_Valid_AllOptionalData => new( DefaultIconPathWithQuotes, SuspendIconPathWithQuotes, ReloadShortcut, SuspendShortcut );
			public static ScriptModuleOptions ModuleOptions_AllReplaceStrings => new( string.Empty, string.Empty, Shortcut_AllReplaceStrings, string.Empty );
			public static string[] RegionData_AllFindStrings => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Common.Syntax.Tokens.LineComment } line comment",
				string.Empty,
				$"\t{ Syntax.Tokens.ReloadShortcut } { Shortcut_AllFindStrings }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
			};
			public static string[] RegionData_UnknownToken => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\tunknown{ Common.Syntax.OperatorChars.TokenValueDivider } token",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_Valid_AllOptionalTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				$"\t{ Common.Syntax.Tokens.LineComment } line comment",
				string.Empty,
				$"\t{ Tokens.DefaultIconFilePath } \"{ DefaultIconPath }\" { Common.Syntax.Tokens.LineComment } inline comment",
				$"\t{ Tokens.SuspendIconFilePath } \"{ SuspendIconPath }\"",
				$"\t{ Tokens.ReloadShortcut } { ReloadShortcut }",
				$"\t{ Tokens.SuspendShortcut } { SuspendShortcut }",
				Common.Syntax.Tokens.RegionClose,
			};
			public static string[] RegionData_Valid_NoOptionalTokens => new[]
			{
				Common.Syntax.Tokens.RegionOpen,
				Common.Syntax.Tokens.RegionClose,
			};
			public static string ReloadShortcut => "reloadshortcut";
			public static string Shortcut_AllFindStrings
				=> $"[windows] [win] [alt] [left-alt] [lalt] [right-alt] [ralt] [control] [ctrl] [left-control] [lctrl] [right-control] [rctrl] [shift] [left-shift] [lshift] [right-shift] [rshift] [and] [alt-graph] [altgr] [wildcard] [wild] [passthrough] [tilde] [send] [tab] [caps-lock] [caps] [enter] [backspace] [bksp] [insert] [ins] [delete] [del] [home] [end] [page-up] [pgup] [page-down] [pgdn] [unknown] { Common.Syntax.OperatorChars.Escape }[del{ Common.Syntax.OperatorChars.Escape }]";
			public static string Shortcut_AllReplaceStrings
				=> "# # ! <! <! >@ >@ ^ ^ <^ <^ >^ >^ + <+ <+ >+ >+ & <^>! <^>! * * ~ ~ $ Tab CapsLock CapsLock Enter Backspace Backspace Insert Insert Delete Delete Home End PageUp PageUp PageDown PageDown [unknown] [del]";
			public static string SuspendIconPath => "path/to/suspendicon.ico";
			public static string SuspendIconPathWithQuotes => $"\"{SuspendIconPath}\"";
			public static string SuspendShortcut => "suspendshortcut";
		}


		public ModuleOptionsRegionParser? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData, ScriptModuleOptions expectedResult )
		{
			var actualResult = parser!.Parse( regionData );
			Assert.AreEqual( expectedResult, actualResult );

			var expectedHasParsedMaxAllowedRegions = true;
			var actualHasParsedMaxAllowedRegions = parser.HasParsedMaxAllowedRegions;
			Assert.AreEqual( expectedHasParsedMaxAllowedRegions, actualHasParsedMaxAllowedRegions );

			var expectedLinesParsed = regionData.Length;
			var actualLinesParsed = parser.LinesParsed;
			Assert.AreEqual( expectedLinesParsed, actualLinesParsed );

			var expectedRegionsParsed = 1;
			var actualRegionsParsed = parser.RegionsParsed;
			Assert.AreEqual( expectedRegionsParsed, actualRegionsParsed );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid_AllOptionalTokens, TestData.ModuleOptions_Valid_AllOptionalData };
				yield return new object[] { TestData.RegionData_Valid_NoOptionalTokens, TestData.ModuleOptions_Valid_NoOptionalData };
				yield return new object[] { TestData.RegionData_AllFindStrings, TestData.ModuleOptions_AllReplaceStrings };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketException( string[] regionData ) => _ = parser!.Parse( regionData );

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
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
			_ = parser!.Parse( TestData.RegionData_Valid_NoOptionalTokens );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_UnknownToken };
			}
		}
	}
}
