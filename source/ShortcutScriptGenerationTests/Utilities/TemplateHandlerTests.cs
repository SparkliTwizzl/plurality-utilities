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
	public class TemplateHandlerTests
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
			public static string FindToken_NoBody => $"{Tokens.Find.Qualify()}";
			public static string FindToken_NoItems => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Common.Syntax.ControlSequences.RegionClose}";
			public static string FindToken_NoRegionClose => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Find1}, {Find2}";
			public static string FindToken_NoRegionOpen => $"{Tokens.Find.Qualify()} {Find1}, {Find2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string FindToken_Valid => $"{Tokens.Find.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Find1}, {Find2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string Replace1 => "replace1";
			public static string Replace2 => "replace2";
			public static string ReplaceToken_NoBody => $"{Tokens.Replace.Qualify()}";
			public static string ReplaceToken_NoItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_NoRegionClose => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2}";
			public static string ReplaceToken_NoRegionOpen => $"{Tokens.Replace.Qualify()} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_TooFewItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1} {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_TooManyItems => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2}, third item {Common.Syntax.ControlSequences.RegionClose}";
			public static string ReplaceToken_Valid => $"{Tokens.Replace.Qualify()} {Common.Syntax.ControlSequences.RegionOpen} {Replace1}, {Replace2} {Common.Syntax.ControlSequences.RegionClose}";
			public static string TemplateFindString => $"{TemplateTriggerString}{TemplateFindTag}";
			public static string TemplateFindTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateFindTagValue => Tokens.Tag.Key;
			public static string TemplateReplaceString => TemplateReplaceTag;
			public static string TemplateReplaceTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateReplaceTagValue => Tokens.Name.Key;
			public static string TemplateToken_DanglingEscapeCharacter
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}";
			public static string TemplateToken_InvalidFindTag
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}invalid{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_NoFindString
				=> $"{Tokens.Template.Qualify()} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagClose
				=> $"{Tokens.Template.Qualify()} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateFindTagValue} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoFindTagOpen
				=> $"{Tokens.Template.Qualify()} {TemplateFindTagValue}{Common.Syntax.ControlSequences.FindTagClose} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceString}";
			public static string TemplateToken_NoReplaceString
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider}";
			public static string TemplateToken_NoReplaceTagClose
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}{TemplateReplaceTagValue}";
			public static string TemplateToken_NoReplaceTagOpen
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateToken_Valid
				=> $"{Tokens.Template.Qualify()} {TemplateFindString} {ControlSequences.TemplateFindReplaceDivider} {TemplateReplaceString} {Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}text{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string TemplateTriggerString => "~";
			public static ScriptMacroTemplate TemplateWithTemplateString => new()
			{
				TemplateString = $"::{TemplateFindString}::{TemplateReplaceString} {Common.Syntax.ControlSequences.FindTagOpenStandin}text{Common.Syntax.ControlSequences.FindTagCloseStandin}",
			};
			public static ScriptMacroTemplate TemplateWithEmptyFindAndReplace => new();
			public static ScriptMacroTemplate TemplateWithFindAndReplace => new()
			{
				FindAndReplace = FindAndReplace,
			};
			public static ScriptMacroTemplate TemplateWithFindKeys => new()
			{
				FindAndReplace = FindAndReplace_KeysOnly,
			};
			public const int TokenStartIndex = 0;
		}


		[TestInitialize]
		public void Setup() => TestUtilities.InitializeLoggingForTests();


		[TestMethod]
		[DynamicData( nameof( FindTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<ScriptMacroTemplate> expected )
		{
			var actual = TemplateHandler.FindTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> FindTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_Valid ), new ProcessedRegionData<ScriptMacroTemplate>( TestData.TemplateWithFindKeys ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoItems ), new ProcessedRegionData<ScriptMacroTemplate>( TestData.TemplateWithEmptyFindAndReplace ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( FindTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void FindTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = TemplateHandler.FindTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> FindTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoBody ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoRegionClose ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.FindToken_NoRegionOpen ) };
			}
		}


		[TestMethod]
		[DynamicData( nameof( ReplaceTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ReplaceTokenHandler_Test_Success( IndexedString[] regionData, ScriptMacroTemplate input, ProcessedRegionData<ScriptMacroTemplate> expected )
		{
			var actual = TemplateHandler.ReplaceTokenHandler( regionData, TestData.TokenStartIndex, result: input );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_Valid ), TestData.TemplateWithFindKeys, new ProcessedRegionData<ScriptMacroTemplate>( TestData.TemplateWithFindAndReplace ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoItems ), TestData.TemplateWithEmptyFindAndReplace, new ProcessedRegionData<ScriptMacroTemplate>( TestData.TemplateWithEmptyFindAndReplace ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void ReplaceTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = TemplateHandler.ReplaceTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> ReplaceTokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoBody ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooFewItems ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_TooManyItems ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionClose ) };
				yield return new object[] { IndexedString.IndexRawStrings( TestData.ReplaceToken_NoRegionOpen ) };
			}
		}


		[TestMethod]
		[DynamicData( nameof( TemplateTokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void TemplateTokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<ScriptMacroTemplate> expected )
		{
			var actual = TemplateHandler.TemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> TemplateTokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { IndexedString.IndexRawStrings( TestData.TemplateToken_Valid ), new ProcessedRegionData<ScriptMacroTemplate>( TestData.TemplateWithTemplateString ) };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( TemplateTokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void TemplateTokenHandler_Test_Throws_TokenValueException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = TemplateHandler.TemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> TemplateTokenHandler_Test_Throws_TokenValueExeception_Data
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
	}
}
