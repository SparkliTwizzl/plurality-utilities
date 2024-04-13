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
			public static Entry[] EntryList => new[]
			{
				new Entry(
					EntryID,
					new List<Identity>(){ new( EntryName, EntryTag ) },
					new Identity( EntryLastName, EntryLastTag ),
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
			public static InputData Input => new( ModuleOptions, EntryList, TemplateList, RawShortcuts );
			public static ModuleOptionData ModuleOptions => new( TestAssets.DefaultIconFileName, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
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
			public static ShortcutData TemplateDataTextCaseDefault => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
			};
			public static ShortcutData TemplateDataTextCaseFirstCaps => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseFirstCaps,
			};
			public static ShortcutData TemplateDataTextCaseLower => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseLower,
			};
			public static ShortcutData TemplateDataTextCaseUnchanged => new()
			{
				FindAndReplace = TemplateFindAndReplace,
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = TemplateReplaceString,
				TextCase = TextCaseUnchanged,
			};
			public static ShortcutData TemplateDataTextCaseUpper => new()
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
			public static ShortcutData[] TemplateList => new[]
			{
				TemplateDataTextCaseDefault,
				TemplateDataTextCaseFirstCaps,
				TemplateDataTextCaseLower,
				TemplateDataTextCaseUnchanged,
				TemplateDataTextCaseUpper,
			};
			public static string TemplateFindString
				=> $"U+005C{TemplateFindTags.Tag}-{TemplateFindTags.LastTag}";
			public static string TemplateReplaceString
				=> $"U+005B{TemplateFindTags.ID}U+005D {TemplateFindTags.Name} {TemplateFindTags.LastName} {TemplateFindTags.Pronoun} {TemplateFindTags.Color} {TemplateFindTags.Decoration} {Find1}{Find2} `";
			public const string TextCaseFirstCaps = TemplateTextCases.FirstCaps;
			public const string TextCaseLower = TemplateTextCases.Lower;
			public const string TextCaseUnchanged = TemplateTextCases.Unchanged;
			public const string TextCaseUpper = TemplateTextCases.Upper;
		}


		public ShortcutProcessor? shortcutProcessor;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			shortcutProcessor = new();
		}


		[TestMethod]
		public void Generate_Test_Success()
		{
			var expected = TestData.Shortcuts;
			var input = shortcutProcessor!.ProcessAndStoreShortcuts( TestData.Input );
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
