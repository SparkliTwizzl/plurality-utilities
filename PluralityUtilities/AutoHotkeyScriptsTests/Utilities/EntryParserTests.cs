using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.TestCommon.Utilities;

namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class EntryParserTests
	{
		public static class TestData
		{
			public static string[] InputData_BlankDecorationField = new string[]
			{
				"{",
				"	{",
				"		% #name# @tag",
				"		&",
				"	}",
				"}",
			};
			public static string[] InputData_BlankNameField = new string[]
			{
				"{",
				"	{",
				"		% ## @tag",
				"	}",
				"}",
			};
			public static string[] InputData_BlankPronounField = new string[]
			{
				"{",
				"	{",
				"		% #name# @tag",
				"		$",
				"	}",
				"}",
			};
			public static string[] InputData_BlankTagField = new string[]
			{
				"{",
				"	{",
				"		% #name# @",
				"	}",
				"}",
			};
			public static string[] InputData_EntryNotClosed = new string[]
			{
				"{",
				"	{",
				"		% #name# @tag",
				"}",
			};
			public static string[] InputData_MissingIdentityField = new string[]
			{
				"{",
				"	{",
				"	}",
				"}",
			};
			public static string[] InputData_MissingNameField = new string[]
			{
				"{",
				"	{",
				"		% @tag",
				"	}",
				"}",
			};
			public static string[] InputData_MissingTagField = new string[]
			{
				"{",
				"	{",
				"		% #name#",
				"	}",
				"}",
			};
			public static string[] InputData_TagFieldContainsSpaces = new string[]
			{
				"{",
				"	{",
				"		% #name# @t ag",
				"	}",
				"}",
			};
			public static string[] InputData_TooManyDecorationFields = new string[]
			{
				"{",
				"	{",
				"		% #name# @tag",
				"		$pronoun",
				"		&decoration",
				"		&decoration",
				"	}",
				"}",
			};
			public static string[] InputData_TooManyPronounFields = new string[]
			{
				"{",
				"	{",
				"		% #name# @tag",
				"		$pronoun",
				"		$pronoun",
				"	}",
				"}",
			};
			public static string[] InputData_UnexpectedCharBetweenEntries = new string[]
			{
				"{",
				"	a",
				"	{",
				"		% #name# @tag",
				"		$pronoun",
				"	}",
				"}",
			};
			public static string[] InputData_UnexpectedCharInEntry = new string[]
			{
				"{",
				"	{",
				"		a",
				"		% #name# @tag",
				"		$pronoun",
				"	}",
				"}",
			};
			public static string[] InputData_Valid = new string[]
			{
				"{",
				"	{",
				"		% #name1# @tag1",
				"		% #name2# @tag2",
				"		$pronoun1",
				"		decoration1",
				"	}",
				"	{",
				"		% #name3# @tag3",
				"		% #name3# @tag3",
				"		$pronoun3",
				"		decoration3",
				"	}",
				"}",
			};
		}


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void ParseEntriesFromDataTest_Success()
		{

		}

		[ TestMethod ]
		[ ExpectedException( typeof( BlankInputFieldException) ) ]
		[ DataRow( "InputParser_BlankDecorationField.txt" ) ]
		[ DataRow( "InputParser_BlankNameField.txt" ) ]
		[ DataRow( "InputParser_BlankPronounField.txt" ) ]
		[ DataRow( "InputParser_BlankTagField.txt" ) ]
		public void ParseInputFileTest_ThrowsBlankInputFieldException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( DuplicateInputFieldException) ) ]
		[ DataRow( "InputParser_TooManyDecorationFields.txt" ) ]
		[ DataRow( "InputParser_TooManyPronounFields.txt" ) ]
		public void ParseInputFileTest_ThrowsDuplicateInputFieldException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InputEntryNotClosedException) ) ]
		[ DataRow( "InputParser_EntryNotClosed.txt" ) ]
		public void ParseInputFileTest_ThrowsInputEntryNotClosedException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InvalidInputFieldException) ) ]
		[ DataRow( "InputParser_TagFieldContainsSpaces.txt" ) ]
		public void ParseInputFileTest_ThrowsInvalidInputFieldException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( MissingInputFieldException) ) ]
		[ DataRow( "InputParser_MissingIdentityField.txt" ) ]
		[ DataRow( "InputParser_MissingNameField.txt" ) ]
		[ DataRow( "InputParser_MissingTagField.txt" ) ]
		public void ParseInputFileTest_ThrowsMissingInputFieldException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( UnexpectedCharacterException) ) ]
		[ DataRow( "InputParser_UnexpectedCharacterBetweenEntries.txt" ) ]
		[ DataRow( "InputParser_UnexpectedCharacterInsideEntry.txt" ) ]
		public void ParseInputFileTest_ThrowsUnexpectedCharacterException( string fileName )
		{
			InputParser.ParseInputFile( TestUtilities.LocateInputFile( fileName ) );
		}
	}
}