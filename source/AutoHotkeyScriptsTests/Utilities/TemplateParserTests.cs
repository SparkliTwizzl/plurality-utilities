using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.AutoHotkeyScripts.Exceptions;
using Petrichor.TestCommon.Utilities;


namespace Petrichor.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class TemplateParserTests
	{
		public static class TestData
		{
			public static readonly string[] RawTemplateData_Valid = new string[]
			{
				"{",
				@"	::\@@:: #",
				@"	::\@\$\&@:: # $ &",
				"}",
			};
			public static readonly string[] RawTemplateData_TrailingEscapeCharacter = new string[]
			{
				"{",
				@"::\@@:: #\",
				"}",
			};
			public static readonly string[] Templates = new string[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public TemplateParser? TemplateParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			TemplateParser = new TemplateParser();
		}


		[ TestMethod ]
		public void ParseTemplatesFromFileTest_Success()
		{
			var expected = TestData.Templates;
			var i = 0;
			var actual = TemplateParser.ParseTemplatesFromData( TestData.RawTemplateData_Valid, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( EscapeCharacterMismatchException ) ) ]
		public void ParseTemplatesFromFileTest_ThrowsEscapeCharacterMismatchException()
		{
			var i = 0;
			_ = TemplateParser.ParseTemplatesFromData( TestData.RawTemplateData_TrailingEscapeCharacter, ref i );
		}
	}
}