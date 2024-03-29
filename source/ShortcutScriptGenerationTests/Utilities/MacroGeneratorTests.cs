using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
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
			public static string EntryID => "iD";
			public static string EntryIDFirstCaps => "Id";
			public static string EntryColor => "coloR";
			public static string EntryColorFirstCaps => "Color";
			public static string EntryDecoration => "decoratioN";
			public static string EntryDecorationFirstCaps => "Decoration";
			public static string EntryName => "namE";
			public static string EntryNameFirstCaps => "Name";
			public static string EntryLastName => "last_namE";
			public static string EntryLastNameFirstCaps => "Last_Name";
			public static string EntryLastTag => "last_taG";
			public static string EntryLastTagFirstCaps => "Last_Tag";
			public static string EntryPronoun => "pronouN";
			public static string EntryPronounFirstCaps => "Pronoun";
			public static string EntryTag => "taG";
			public static string EntryTagFirstCaps => "Tag";
			public static ScriptInput Input => new( ModuleOptions, EntryList, TemplateList );
			public static string[] Macros => new[]
			{
				MacroTextCaseDefault,
				MacroTextCaseFirstCaps,
				MacroTextCaseLower,
				MacroTextCaseUnchanged,
				MacroTextCaseUpper,
			};
			public static string MacroTextCaseDefault => MacroTextCaseUnchanged;
			public static string MacroTextCaseFirstCaps
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::{Common.Syntax.ControlSequences.FindTagOpen}{EntryIDFirstCaps}{Common.Syntax.ControlSequences.FindTagClose} {EntryNameFirstCaps} {EntryLastNameFirstCaps} {EntryPronounFirstCaps} {EntryColorFirstCaps} {EntryDecorationFirstCaps} {TemplateReplace1FirstCaps}{TemplateReplace2FirstCaps} `";
			public static string MacroTextCaseLower
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::" + $"{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {TemplateReplace1}{TemplateReplace2} `".ToLower();
			public static string MacroTextCaseUnchanged
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {TemplateReplace1}{TemplateReplace2} `";
			public static string MacroTextCaseUpper
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::" + $"{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {TemplateReplace1}{TemplateReplace2} `".ToUpper();
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFileName, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static ScriptMacroTemplate TemplateTextCaseDefault => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
			};
			public static ScriptMacroTemplate TemplateTextCaseFirstCaps => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseFirstCaps,
			};
			public static ScriptMacroTemplate TemplateTextCaseLower => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseLower,
			};
			public static ScriptMacroTemplate TemplateTextCaseUnchanged => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseUnchanged,
			};
			public static ScriptMacroTemplate TemplateTextCaseUpper => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseUpper,
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
				TemplateTextCaseDefault,
				TemplateTextCaseFirstCaps,
				TemplateTextCaseLower,
				TemplateTextCaseUnchanged,
				TemplateTextCaseUpper,
			};
			public static string TemplateReplace1 => "replace1";
			public static string TemplateReplace1FirstCaps => "Replace1";
			public static string TemplateReplace2 => "replace2";
			public static string TemplateReplace2FirstCaps => "Replace2";
			public static string TemplateFindString
				=> $"{Common.Syntax.ControlSequences.EscapeStandin}{TemplateFindTags.Tag}-{TemplateFindTags.LastTag}";
			public static string TemplateReplaceString
				=> $"{Common.Syntax.ControlSequences.FindTagOpenStandin}{TemplateFindTags.ID}{Common.Syntax.ControlSequences.FindTagCloseStandin} {TemplateFindTags.Name} {TemplateFindTags.LastName} {TemplateFindTags.Pronoun} {TemplateFindTags.Color} {TemplateFindTags.Decoration} {TemplateFind1}{TemplateFind2} `";
			public const string TextCaseFirstCaps = TemplateTextCases.FirstCaps;
			public const string TextCaseLower = TemplateTextCases.Lower;
			public const string TextCaseUnchanged = TemplateTextCases.Unchanged;
			public const string TextCaseUpper = TemplateTextCases.Upper;
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

			Log.Info( "expected:" );
			foreach ( var item in expected )
			{
				Log.Info( $"    {item}" );
			}

			Log.Info( "actual:" );
			foreach ( var item in actual )
			{
				Log.Info( $"    {item}" );
			}

			CollectionAssert.AreEqual( expected, actual );
		}
	}
}
