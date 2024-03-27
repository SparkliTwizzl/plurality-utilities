using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class TemplateHandlerTests
	{
		public readonly struct TestData
		{
			public static string FindString => $"{TriggerString}{FindTag}";
			public static string FindTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{FindTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string FindTagValue => Tokens.Tag.Key;
			public static ProcessedRegionData<List<string>> ProcessedTemplates => new()
			{
				Value = Templates.ToList(),
			};
			public static IndexedString[] RegionData_DanglingEscapeCharacter => IndexedString.IndexStringArray( new[] { Token_DanglingEscapeCharacter } );
			public static IndexedString[] RegionData_InvalidFindTag => IndexedString.IndexStringArray( new[] { Token_InvalidFindTag } );
			public static IndexedString[] RegionData_NoFindString => IndexedString.IndexStringArray( new[] { Token_NoFindString } );
			public static IndexedString[] RegionData_NoFindTagClose => IndexedString.IndexStringArray( new[] { Token_NoFindTagClose } );
			public static IndexedString[] RegionData_NoFindTagOpen => IndexedString.IndexStringArray( new[] { Token_NoFindTagOpen } );
			public static IndexedString[] RegionData_NoReplaceString => IndexedString.IndexStringArray( new[] { Token_NoReplaceString } );
			public static IndexedString[] RegionData_NoReplaceTagClose => IndexedString.IndexStringArray( new[] { Token_NoReplaceTagClose } );
			public static IndexedString[] RegionData_NoReplaceTagOpen => IndexedString.IndexStringArray( new[] { Token_NoReplaceTagOpen } );
			public static IndexedString[] RegionData_Valid => IndexedString.IndexStringArray( new[] { Token_Valid } );
			public static string ReplaceString => ReplaceTag;
			public static string ReplaceTag
				=> $"{Common.Syntax.ControlSequences.FindTagOpen}{ReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string ReplaceTagValue => Tokens.Name.Key;
			public static string TemplateTokenString => Tokens.Template.Qualify();
			public static string[] Templates => new[]
			{
				$"::{FindString}::{ReplaceString} {Common.Syntax.ControlSequences.FindTagOpenStandin}text{Common.Syntax.ControlSequences.FindTagCloseStandin}",
			};
			public static string TriggerString => "~";
			public static string Token_DanglingEscapeCharacter
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider} {ReplaceString} {Common.Syntax.ControlSequences.Escape}";
			public static string Token_InvalidFindTag
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}invalid{Common.Syntax.ControlSequences.FindTagClose}";
			public static string Token_NoFindString
				=> $"{TemplateTokenString} {ControlSequences.TemplateFindReplaceDivider} {ReplaceString}";
			public static string Token_NoFindTagClose
				=> $"{TemplateTokenString} {Common.Syntax.ControlSequences.FindTagOpen}{FindTagValue} {ControlSequences.TemplateFindReplaceDivider} {ReplaceString}";
			public static string Token_NoFindTagOpen
				=> $"{TemplateTokenString} {FindTagValue}{Common.Syntax.ControlSequences.FindTagClose} {ControlSequences.TemplateFindReplaceDivider} {ReplaceString}";
			public static string Token_NoReplaceString
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider}";
			public static string Token_NoReplaceTagClose
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider} {Common.Syntax.ControlSequences.FindTagOpen}{ReplaceTagValue}";
			public static string Token_NoReplaceTagOpen
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider} {ReplaceTagValue}{Common.Syntax.ControlSequences.FindTagClose}";
			public static string Token_Valid
				=> $"{TemplateTokenString} {FindString} {ControlSequences.TemplateFindReplaceDivider} {ReplaceString} {Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}text{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}";
			public const int TokenStartIndex = 0;
		}


		[TestInitialize]
		public void Setup() => TestUtilities.InitializeLoggingForTests();


		[TestMethod]
		[DynamicData( nameof( TokenHandler_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void TokenHandler_Test_Success( IndexedString[] regionData, ProcessedRegionData<List<string>> expected )
		{
			var actual = TemplateHandler.TemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
			Assert.AreEqual( expected.BodySize, actual.BodySize );
			CollectionAssert.AreEqual( expected.Value, actual.Value );
		}

		public static IEnumerable<object[]> TokenHandler_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid, TestData.ProcessedTemplates };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenValueException ) )]
		[DynamicData( nameof( TokenHandler_Test_Throws_TokenValueExeception_Data ), DynamicDataSourceType.Property )]
		public void TokenHandler_Test_Throws_TokenCountException( IndexedString[] regionData )
		{
			Log.Info( $"input: {regionData[ 0 ]}" );
			_ = TemplateHandler.TemplateTokenHandler( regionData, TestData.TokenStartIndex, result: new() );
		}

		public static IEnumerable<object[]> TokenHandler_Test_Throws_TokenValueExeception_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingEscapeCharacter };
				yield return new object[] { TestData.RegionData_InvalidFindTag };
				yield return new object[] { TestData.RegionData_NoFindString };
				yield return new object[] { TestData.RegionData_NoFindTagClose };
				yield return new object[] { TestData.RegionData_NoFindTagOpen };
				yield return new object[] { TestData.RegionData_NoReplaceString };
				yield return new object[] { TestData.RegionData_NoReplaceTagClose };
				yield return new object[] { TestData.RegionData_NoReplaceTagOpen };
			}
		}
	}
}
