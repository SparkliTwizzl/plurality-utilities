using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class MacroGeneratorTests
	{
		public readonly struct TestData
		{
			public static ScriptEntry[] EntryList => new[]
			{
				new ScriptEntry(
					EntryID,
					new List<ScriptIdentity>(){ new( EntryName, EntryTag ) },
					new ScriptIdentity( EntryLastName, EntryLastTag ),
					EntryPronoun,
					EntryColor,
					EntryDecoration
				),
			};
			public static string EntryID => "ID";
			public static string EntryColor => "COLOR";
			public static string EntryDecoration => "DECORATION";
			public static string EntryName => "NAME";
			public static string EntryLastName => "LAST_NAME";
			public static string EntryLastTag => "LAST_TAG";
			public static string EntryPronoun => "PRONOUN";
			public static string EntryTag => "TAG";
			public static ScriptInput Input => new( ModuleOptions, EntryList, TemplateList );
			public static string[] Macros => new[]
			{
				$"::{ Common.Syntax.ControlSequences.Escape }{ EntryTag }{ EntryLastTag }:: { Common.Syntax.ControlSequences.FindTagOpen }{ EntryID }{ Common.Syntax.ControlSequences.FindTagClose } { EntryName } { EntryLastName } { EntryPronoun } { EntryColor } { EntryDecoration } {TemplateReplace1}{TemplateReplace2} `",
			};
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFileName, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static ScriptMacroTemplate Template => new()
			{
				TemplateString = TemplateString,
				FindAndReplace = TemplateFindAndReplace,
			};
			public static string TemplateFind1 => "find1";
			public static string TemplateFind2 => "find2";
			public static Dictionary<string, string> TemplateFindAndReplace => new()
			{
				{ TemplateFind1, TemplateReplace1 },
				{ TemplateFind2, TemplateReplace2 },
			};
			public static ScriptMacroTemplate[] TemplateList => new[]
			{
				Template,
			};
			public static string TemplateReplace1 => "replace1";
			public static string TemplateReplace2 => "replace2";
			public static string TemplateString => $"::{Common.Syntax.ControlSequences.EscapeStandin}{TemplateFindStrings.Tag}{TemplateFindStrings.LastTag}:: {Common.Syntax.ControlSequences.FindTagOpenStandin}{TemplateFindStrings.ID}{Common.Syntax.ControlSequences.FindTagCloseStandin} {TemplateFindStrings.Name} {TemplateFindStrings.LastName} {TemplateFindStrings.Pronoun} {TemplateFindStrings.Color} {TemplateFindStrings.Decoration} {TemplateFind1}{TemplateFind2} `";
		}


		public MacroGenerator? generator;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			generator = new();
		}


		[TestMethod]
		public void Generate_Test_Success()
		{
			var expected = TestData.Macros;
			var actual = generator!.Generate( TestData.Input );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}
