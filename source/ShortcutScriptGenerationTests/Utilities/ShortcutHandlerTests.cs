using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutHandlerTests
	{
		public readonly struct TestData
		{
			public static string Find1 => "find1";
			public static string Find2 => "find2";
			public static Dictionary<string, string> FindAndReplace => new()
			{
				{ Find1, Replace1 },
				{ Find2, Replace2 },
			};
			public static Dictionary<string, string> FindAndReplace_Empty => new();
			public static Dictionary<string, string> FindAndReplace_KeysOnly => new()
			{
				{ Find1, string.Empty },
				{ Find2, string.Empty },
			};
			public static string FindToken_NoItems => $"{TokenPrototypes.Find.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string FindToken_NoValue => $"{TokenPrototypes.Find.QualifyKey()}";
			public static string FindToken_NoRegionClose => $"{TokenPrototypes.Find.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Find1}, {Find2}";
			public static string FindToken_NoRegionOpen => $"{TokenPrototypes.Find.QualifyKey()} {Find1}, {Find2} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string FindToken_Valid => $"{TokenPrototypes.Find.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Find1}, {Find2} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string Replace1 => "replace1";
			public static string Replace2 => "replace2";
			public static string ReplaceToken_NoItems => $"{TokenPrototypes.Replace.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string ReplaceToken_NoRegionClose => $"{TokenPrototypes.Replace.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Replace1}, {Replace2}";
			public static string ReplaceToken_NoRegionOpen => $"{TokenPrototypes.Replace.QualifyKey()} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string ReplaceToken_NoValue => $"{TokenPrototypes.Replace.QualifyKey()}";
			public static string ReplaceToken_TooFewItems => $"{TokenPrototypes.Replace.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Replace1} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string ReplaceToken_TooManyItems => $"{TokenPrototypes.Replace.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Replace1}, {Replace2}, third item {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string ReplaceToken_Valid => $"{TokenPrototypes.Replace.QualifyKey()} {Common.Syntax.ControlSequences.TokenBodyOpen} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.TokenBodyClose}";
			public static string ShortcutString => $"find{ControlSequences.TemplateFindStringDelimiter}replace";
			public static string ShortcutToken_Malformed => $"{TokenPrototypes.Shortcut.QualifyKey()}invalid";
			public static string ShortcutToken_NoValue => $"{TokenPrototypes.Shortcut.QualifyKey()}";
			public static string ShortcutToken_Valid => $"{TokenPrototypes.Shortcut.QualifyKey()}{ShortcutString}";
			public static string TemplateFindString => $"{TemplateTriggerString}{TemplateFindTag}";
			public static string TemplateFindTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateFindTagValue => TokenPrototypes.Tag.Key;
			public static string TemplateReplaceString => TemplateReplaceTag;
			public static string TemplateReplaceTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateReplaceTagValue => TokenPrototypes.Name.Key;
			public const string TemplateTextCase = TemplateTextCases.Upper;
			public static string TemplateToken_DanglingEscapeCharacter
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}";
			public static string TemplateToken_InvalidFindTag
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter} {Common.Syntax.ControlSequences.FindTagOpen}invalid{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_NoFindString
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagClose
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagOpen
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceString}";
			public static string TemplateToken_NoReplaceString
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter}";
			public static string TemplateToken_NoReplaceTagClose
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}";
			public static string TemplateToken_NoReplaceTagOpen
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_Valid
				=> $"{TokenPrototypes.ShortcutTemplate.QualifyKey()} {TemplateFindString} {ControlSequences.TemplateFindStringDelimiter} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}text{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateTriggerString => "~";
			public static TextShortcut TemplateWithEmptyFindAndReplace => new();
			public static TextShortcut TemplateWithFindAndReplace => new()
			{
				FindAndReplace = FindAndReplace,
			};
			public static TextShortcut TemplateWithFindKeys => new()
			{
				FindAndReplace = FindAndReplace_KeysOnly,
			};
			public static TextShortcut TemplateWithTemplateString => new()
			{
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = $"{TemplateReplaceString} U+005BtextU+005D",
			};
			public static TextShortcut TemplateWithTextCase => new()
			{
				TextCase = TemplateTextCase,
			};
			public static string TextCaseToken_NoValue => $"{TokenPrototypes.TextCase.QualifyKey()}";
			public static string TextCaseToken_UnrecognizedValue => $"{TokenPrototypes.TextCase.QualifyKey()} invalid";
			public static string TextCaseToken_Valid => $"{TokenPrototypes.TextCase.QualifyKey()} {TemplateTextCase}";
			public const int TokenStartIndex = 0;
		}


		[TestInitialize]
		public void Setup() => TestUtilities.InitializeLoggingForTests();


		[TestMethod]
		[DynamicData( nameof( FindTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Success( IndexedString[] bodyData, ProcessedTokenData<TextShortcut> expected )
		{
			var actual = ShortcutHandler.FindTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> FindTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_Valid ), new ProcessedTokenData<TextShortcut>( TestData.TemplateWithFindKeys ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( FindTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Throws_TokenValueException( IndexedString[] bodyData )
		{
			Logger.Info( $"input: {bodyData[ 0 ]}" );
			_ = ShortcutHandler.FindTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> FindTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoItems ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoValue ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoRegionClose ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoRegionOpen ) };
			}
		}


		[TestMethod]
		[DynamicData( nameof( ReplaceTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ReplaceTokenHandler_Test_Success( IndexedString[] bodyData, TextShortcut input, ProcessedTokenData<TextShortcut> expected )
		{
			var actual = ShortcutHandler.ReplaceTokenHandler( bodyData, TestData.TokenStartIndex, result: input );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_Valid ), TestData.TemplateWithFindKeys, new ProcessedTokenData<TextShortcut>( TestData.TemplateWithFindAndReplace ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ReplaceTokenHandler_Test_Throws_TokenValueException( IndexedString[] bodyData, TextShortcut input )
		{
			Logger.Info( $"input: {bodyData[ 0 ]}" );
			_ = ShortcutHandler.ReplaceTokenHandler( bodyData, TestData.TokenStartIndex, result: input );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoItems ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionClose ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionOpen ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoValue ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooFewItems ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooManyItems ), new TextShortcut() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_Valid ), TestData.TemplateWithEmptyFindAndReplace };
			}
		}


		[TestMethod]
		[DynamicData( nameof( ShortcutTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTokenHandler_Test_Success( IndexedString[] bodyData, ProcessedTokenData<ShortcutScriptInput> expected )
		{
			var actual = ShortcutHandler.ShortcutTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ShortcutTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ShortcutToken_Valid ), new ProcessedTokenData<ShortcutScriptInput>() { Value = new() { Shortcuts = [TestData.ShortcutString] } } };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ShortcutTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTokenHandler_Test_Throws_TokenValueException( IndexedString[] bodyData )
		{
			Logger.Info( $"input: {bodyData[ 0 ]}" );
			_ = ShortcutHandler.ShortcutTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> ShortcutTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ShortcutToken_Malformed ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ShortcutToken_NoValue ) };
			}
		}


		[TestMethod]
		[DynamicData( nameof( ShortcutTemplateTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTemplateTokenHandler_Test_Success( IndexedString[] bodyData, ProcessedTokenData<TextShortcut> expected )
		{
			var actual = ShortcutHandler.ShortcutTemplateTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ShortcutTemplateTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_Valid ), new ProcessedTokenData<TextShortcut>( TestData.TemplateWithTemplateString ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ShortcutTemplateTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTemplateTokenHandler_Test_Throws_TokenValueException( IndexedString[] bodyData )
		{
			Logger.Info( $"input: {bodyData[ 0 ]}" );
			_ = ShortcutHandler.ShortcutTemplateTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> ShortcutTemplateTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_DanglingEscapeCharacter ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_InvalidFindTag ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoFindString ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoFindTagClose ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoFindTagOpen ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoReplaceString ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoReplaceTagClose ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_NoReplaceTagOpen ) };
			}
		}


		[TestMethod]
		[DynamicData( nameof( TextCaseTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void TextCaseTokenHandler_Test_Success( IndexedString[] bodyData, ProcessedTokenData<TextShortcut> expected )
		{
			var actual = ShortcutHandler.TextCaseTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> TextCaseTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TextCaseToken_Valid ), new ProcessedTokenData<TextShortcut>( TestData.TemplateWithTextCase ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( TextCaseTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void TextCaseTokenHandler_Test_Throws_TokenValueException( IndexedString[] bodyData )
		{
			Logger.Info( $"input: {bodyData[ 0 ]}" );
			_ = ShortcutHandler.TextCaseTokenHandler( bodyData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> TextCaseTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TextCaseToken_NoValue ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TextCaseToken_UnrecognizedValue ) };
			}
		}
	}
}
