using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[ TestClass ]
	public class ShortcutScriptTemplateParserTests
	{
		public struct TestData
		{
			public static string[] ValidTemplates => new[]
			{
				"{",
				@"	::\@@:: #",
				@"	::\@\$\&@:: # $ &",
				"}",
			};
			public static string[] TemplateWithTrailingExcapeCharacter => new[]
			{
				"{",
				@"	::\@@:: #\",
				"}",
			};
			public static string[] ParsedTemplates => new[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public ShortcutScriptTemplateParser? TemplateParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			TemplateParser = new ShortcutScriptTemplateParser();
		}


		[ TestMethod ]
		public void ParseTemplatesFromFileTest_Success()
		{
			var expected = TestData.ParsedTemplates;
			var i = 0;
			var actual = TemplateParser!.ParseTemplatesFromData( TestData.ValidTemplates, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( EscapeCharacterMismatchException ) ) ]
		public void ParseTemplatesFromFileTest_ThrowsEscapeCharacterMismatchException()
		{
			var i = 0;
			_ = TemplateParser!.ParseTemplatesFromData( TestData.TemplateWithTrailingExcapeCharacter, ref i );
		}
	}
}