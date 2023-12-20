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


		public int i;
		public ShortcutScriptTemplateParser? templateParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 0;
			templateParser = new ShortcutScriptTemplateParser();
		}


		[ TestMethod ]
		public void ParseTemplatesFromFileTest_Success()
		{
			var expected = TestData.ParsedTemplates;
			var actual = templateParser!.ParseTemplatesFromData( TestData.ValidTemplates, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( EscapeCharacterMismatchException ) ) ]
		public void ParseTemplatesFromFileTest_ThrowsEscapeCharacterMismatchException()
		{
			_ = templateParser!.ParseTemplatesFromData( TestData.TemplateWithTrailingExcapeCharacter, ref i );
		}
	}
}