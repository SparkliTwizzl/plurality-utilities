using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class TemplatesRegionParserTests
	{
		public struct TestData
		{
			public static string[] ValidTemplates => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				string.Empty,
				"\t::\\@@:: #",
				$"\t::\\@\\$\\&@:: # $ & { CommonSyntax.LineCommentToken } inline comment",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] TemplateWithTrailingExcapeCharacter => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t::\\@@:: #\\",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] ParsedTemplates => new[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public int i;
		public TemplatesRegionParser? templatesRegionParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 0;
			templatesRegionParser = new TemplatesRegionParser();
		}


		[TestMethod]
		public void ParseTemplatesFromFile_Test_Success()
		{
			var expected = TestData.ParsedTemplates;
			var actual = templatesRegionParser!.ParseTemplatesFromData( TestData.ValidTemplates, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( EscapeCharacterMismatchException ) )]
		public void ParseTemplatesFromFile_Test_ThrowsEscapeCharacterMismatchException()
			=> _ = templatesRegionParser!.ParseTemplatesFromData( TestData.TemplateWithTrailingExcapeCharacter, ref i );
	}
}
