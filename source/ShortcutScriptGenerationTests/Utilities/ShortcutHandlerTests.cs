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
			public static string FindToken_NoItems => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Common.Syntax.ControlSequences.RegionClose}";
			public static string FindToken_NoValue => $"{Tokens.Find.Qualify()}";
			public static string FindToken_NoRegionClose => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Find1}, {Find2}";
			public static string FindToken_NoRegionOpen => $"{Tokens.Find.Qualify()} {Find1}, {Find2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string FindToken_Valid => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Find1}, {Find2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string Replace1 => "replace1";
			public static string Replace2 => "replace2";
			public static string ReplaceToken_NoItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_NoRegionClose => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2}";
			public static string ReplaceToken_NoRegionOpen => $"{Tokens.Replace.Qualify()} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_NoValue => $"{Tokens.Replace.Qualify()}";
			public static string ReplaceToken_TooFewItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_TooManyItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2}, third item {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_Valid => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ShortcutString => $"find{ControlSequences.ShortcutFindReplaceDivider}replace";
			public static string ShortcutToken_Malformed => $"{Tokens.Shortcut.Qualify()}invalid";
			public static string ShortcutToken_NoValue => $"{Tokens.Shortcut.Qualify()}";
			public static string ShortcutToken_Valid => $"{Tokens.Shortcut.Qualify()}{ShortcutString}";
			public static string TemplateFindString => $"{TemplateTriggerString}{TemplateFindTag}";
			public static string TemplateFindTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateFindTagValue => Tokens.Tag.Key;
			public static string TemplateReplaceString => TemplateReplaceTag;
			public static string TemplateReplaceTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateReplaceTagValue => Tokens.Name.Key;
			public const string TemplateTextCase = TemplateTextCases.Upper;
			public static string TemplateToken_DanglingEscapeCharacter
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}";
			public static string TemplateToken_InvalidFindTag
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}invalid{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_NoFindString
				=> $"{Tokens.ShortcutTemplate.Qualify()} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagClose
				=> $"{Tokens.ShortcutTemplate.Qualify()} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagOpen
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoReplaceString
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider}";
			public static string TemplateToken_NoReplaceTagClose
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}";
			public static string TemplateToken_NoReplaceTagOpen
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_Valid
				=> $"{Tokens.ShortcutTemplate.Qualify()} {TemplateFindString} {ControlSequences.ShortcutFindReplaceDivider} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}text{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateTriggerString => "~";
			public static ShortcutData TemplateWithEmptyFindAndReplace => new();
			public static ShortcutData TemplateWithFindAndReplace => new()
			{
				FindAndReplace = FindAndReplace,
			};
			public static ShortcutData TemplateWithFindKeys => new()
			{
				FindAndReplace = FindAndReplace_KeysOnly,
			};
			public static ShortcutData TemplateWithTemplateString => new()
			{
				TemplateFindString = TemplateFindString,
				TemplateReplaceString = $"{TemplateReplaceString} U+005BtextU+005D",
			};
			public static ShortcutData TemplateWithTextCase => new()
			{
				TextCase = TemplateTextCase,
			};
			public static string TextCaseToken_NoValue => $"{Tokens.TextCase.Qualify()}";
			public static string TextCaseToken_UnrecognizedValue => $"{Tokens.TextCase.Qualify()} invalid";
			public static string TextCaseToken_Valid => $"{Tokens.TextCase.Qualify()} {TemplateTextCase}";
			public const int TokenStartIndex = 0;
		}


		[TestInitialize]
		public void Setup() => TestUtilities.InitializeLoggingForTests();


		[TestMethod]
		[DynamicData( nameof( FindTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<ShortcutData> expected )
		{
			var actual = ShortcutHandler.FindTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> FindTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_Valid ), new ProcessedRegionData<ShortcutData>( TestData.TemplateWithFindKeys ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( FindTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = ShortcutHandler.FindTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
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
		public void ReplaceTokenHandler_Test_Success( IndexedString[] regionData, ShortcutData input, ProcessedRegionData<ShortcutData> expected )
		{
			var actual = ShortcutHandler.ReplaceTokenHandler( regionData, TestData.TokenStartIndex, result: input );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_Valid ), TestData.TemplateWithFindKeys, new ProcessedRegionData<ShortcutData>( TestData.TemplateWithFindAndReplace ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ReplaceTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData, ShortcutData input )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = ShortcutHandler.ReplaceTokenHandler( regionData, TestData.TokenStartIndex, result: input );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoItems ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionClose ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionOpen ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoValue ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooFewItems ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooManyItems ), new ShortcutData() };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_Valid ), TestData.TemplateWithEmptyFindAndReplace };
			}
		}


		[TestMethod]
		[DynamicData( nameof( ShortcutTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<InputData> expected )
		{
			var actual = ShortcutHandler.ShortcutTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ShortcutTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ShortcutToken_Valid ), new ProcessedRegionData<InputData>() { Value = new() { Shortcuts = [TestData.ShortcutString] } } };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ShortcutTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = ShortcutHandler.ShortcutTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
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
		public void ShortcutTemplateTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<ShortcutData> expected )
		{
			var actual = ShortcutHandler.ShortcutTemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ShortcutTemplateTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_Valid ), new ProcessedRegionData<ShortcutData>( TestData.TemplateWithTemplateString ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ShortcutTemplateTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ShortcutTemplateTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = ShortcutHandler.ShortcutTemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
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
		public void TextCaseTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<ShortcutData> expected )
		{
			var actual = ShortcutHandler.TextCaseTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> TextCaseTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TextCaseToken_Valid ), new ProcessedRegionData<ShortcutData>( TestData.TemplateWithTextCase ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( TextCaseTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void TextCaseTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = ShortcutHandler.TextCaseTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
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
