using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutProcessorTests
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
			public static string Find1 => "find1";
			public static string Find2 => "find2";
			public static ScriptInput Input => new( ModuleOptions, EntryList, TemplateList, RawShortcuts );
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFileName, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static string RawShortcut => $"{Find1}{ControlSequences.ShortcutFindReplaceDivider}{Replace1}";
			public static string[] RawShortcuts => new[]
			{
				RawShortcut,
			};
			public static string Replace1 => "replace1";
			public static string Replace1FirstCaps => "Replace1";
			public static string Replace2 => "replace2";
			public static string Replace2FirstCaps => "Replace2";
			public static string ShortcutString => $"::{Find1}::{Replace1}";
			public static string[] Shortcuts => new[]
			{
				ShortcutString,
				TemplatedShortcutTextCaseDefault,
				TemplatedShortcutTextCaseFirstCaps,
				TemplatedShortcutTextCaseLower,
				TemplatedShortcutTextCaseUnchanged,
				TemplatedShortcutTextCaseUpper,
			};
			public static ScriptShortcutData TemplateDataTextCaseDefault => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
			};
			public static ScriptShortcutData TemplateDataTextCaseFirstCaps => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseFirstCaps,
			};
			public static ScriptShortcutData TemplateDataTextCaseLower => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseLower,
			};
			public static ScriptShortcutData TemplateDataTextCaseUnchanged => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseUnchanged,
			};
			public static ScriptShortcutData TemplateDataTextCaseUpper => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseUpper,
			};
			public static string TemplatedShortcutTextCaseDefault => TemplatedShortcutTextCaseUnchanged;
			public static string TemplatedShortcutTextCaseFirstCaps
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::{Common.Syntax.ControlSequences.FindTagOpen}{EntryIDFirstCaps}{Common.Syntax.ControlSequences.FindTagClose} {EntryNameFirstCaps} {EntryLastNameFirstCaps} {EntryPronounFirstCaps} {EntryColorFirstCaps} {EntryDecorationFirstCaps} {Replace1FirstCaps}{Replace2FirstCaps} `";
			public static string TemplatedShortcutTextCaseLower
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::" + $"{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {Replace1}{Replace2} `".ToLower();
			public static string TemplatedShortcutTextCaseUnchanged
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {Replace1}{Replace2} `";
			public static string TemplatedShortcutTextCaseUpper
				=> $"::{Common.Syntax.ControlSequences.Escape}{EntryTag}-{EntryLastTag}::" + $"{Common.Syntax.ControlSequences.FindTagOpen}{EntryID}{Common.Syntax.ControlSequences.FindTagClose} {EntryName} {EntryLastName} {EntryPronoun} {EntryColor} {EntryDecoration} {Replace1}{Replace2} `".ToUpper();
			public static Dictionary<string, string> TemplateFindAndReplace => new()
			{
				{ Find1, Replace1 },
				{ Find2, Replace2 },
			};
			public static ScriptShortcutData[] TemplateList => new[]
			{
				TemplateDataTextCaseDefault,
				TemplateDataTextCaseFirstCaps,
				TemplateDataTextCaseLower,
				TemplateDataTextCaseUnchanged,
				TemplateDataTextCaseUpper,
			};
			public static string TemplateFindString
				=> $"{Common.Syntax.ControlSequences.EscapeStandin}{TemplateFindTags.Tag}-{TemplateFindTags.LastTag}";
			public static string TemplateReplaceString
				=> $"{Common.Syntax.ControlSequences.FindTagOpenStandin}{TemplateFindTags.ID}{Common.Syntax.ControlSequences.FindTagCloseStandin} {TemplateFindTags.Name} {TemplateFindTags.LastName} {TemplateFindTags.Pronoun} {TemplateFindTags.Color} {TemplateFindTags.Decoration} {Find1}{Find2} `";
			public const string TextCaseFirstCaps = TemplateTextCases.FirstCaps;
			public const string TextCaseLower = TemplateTextCases.Lower;
			public const string TextCaseUnchanged = TemplateTextCases.Unchanged;
			public const string TextCaseUpper = TemplateTextCases.Upper;
		}


		public ShortcutProcessor? generator;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			generator = new();
		}


		[TestMethod]
		public void Generate_Test_Success()
		{
			var expected = TestData.Shortcuts;
			var input = generator!.ProcessAndStoreShortcuts( TestData.Input );
			var actual = input.Shortcuts;

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
