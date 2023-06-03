using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class TemplateParserTests
	{
		public static class TestData
		{
			public static readonly Entry[] Entries = new Entry[]
			{
				new Entry( new List<Identity>(){ new Identity( "name", "tag" ) }, "pronoun", "decoration" ),
			};
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
			public static readonly string[] ParsedTemplates = new string[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
			public static readonly Input Input = new Input( Entries, ParsedTemplates );
			public static readonly string[] GeneratedMacros = new string[]
			{
				"::@tag:: name",
				"::@$&tag:: name pronoun decoration",
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
		public void GenerateMacrosFromTemplatesTest_Success()
		{
			var expected = TestData.GeneratedMacros;
			var actual = TemplateParser.GenerateMacrosFromInput( TestData.Input ).ToArray();

			Log.WriteLine( "expected:" );
			foreach ( var line in expected )
			{
				Log.WriteLine( $"[{ line }]" );
			}
			Log.WriteLine();
			Log.WriteLine( "actual:" );
			foreach ( var line in actual )
			{
				Log.WriteLine( $"[{ line }]" );
			}

			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		public void ParseTemplatesFromFileTest_Success()
		{
			var expected = TestData.ParsedTemplates;
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