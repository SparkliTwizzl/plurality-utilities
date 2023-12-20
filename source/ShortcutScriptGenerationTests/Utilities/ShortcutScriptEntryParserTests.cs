using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.Logging;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[ TestClass ]
	public class ShortcutScriptEntryParserTests
	{
		public struct TestData
		{
			public const string InputData_BlankDecorationField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		&|" +
				"	}|" +
				"}|";
			public const string InputData_BlankNameField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% ## @tag|" +
				"	}|" +
				"}|";
			public const string InputData_BlankPronounField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$|" +
				"	}|" +
				"}|";
			public const string InputData_BlankTagField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @|" +
				"	}|" +
				"}|";
			public const string InputData_EntryNotClosed =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"}|";
			public const string InputData_MissingIdentityField =
				"entries:|" +
				"{|" +
				"	{|" +
				"	}|" +
				"}|";
			public const string InputData_MissingNameField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% @tag|" +
				"	}|" +
				"}|";
			public const string InputData_MissingTagField =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name#|" +
				"	}|" +
				"}|";
			public const string InputData_TagFieldContainsSpaces =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @t ag|" +
				"	}|" +
				"}|";
			public const string InputData_TooManyDecorationFields =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"		&decoration|" +
				"		&decoration|" +
				"	}|" +
				"}|";
			public const string InputData_TooManyPronounFields =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_UnexpectedCharBetweenEntries =
				"entries:|" +
				"{|" +
				"	a|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_UnexpectedCharInEntry =
				"entries:|" +
				"{|" +
				"	{|" +
				"		a|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_Valid =
				"entries:|" +
				"{|" +
				"	{|" +
				"		% #name1# @tag1|" +
				"		% #name2# @tag2|" +
				"		$pronoun1|" +
				"		&decoration1|" +
				"	}|" +
				"	{|" +
				"		% #name3# @tag3|" +
				"		% #name4# @tag4|" +
				"		$pronoun3|" +
				"		&decoration3|" +
				"	}|" +
				"}|";
		}


		public int i;
		public ShortcutScriptEntryParser? EntryParser;


		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 1;
			EntryParser = new ShortcutScriptEntryParser();
		}


		[ TestMethod ]
		[ DataRow( TestData.InputData_Valid ) ]
		public void ParseEntriesFromDataTest_Success( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( BlankInputFieldException) ) ]
		[ DataRow( TestData.InputData_BlankDecorationField ) ]
		[ DataRow( TestData.InputData_BlankNameField ) ]
		[ DataRow( TestData.InputData_BlankPronounField ) ]
		[ DataRow( TestData.InputData_BlankTagField ) ]
		public void ParseEntriesFromDataTest_ThrowsBlankInputFieldException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( DuplicateInputFieldException) ) ]
		[ DataRow( TestData.InputData_TooManyDecorationFields ) ]
		[ DataRow( TestData.InputData_TooManyPronounFields ) ]
		public void ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InputEntryNotClosedException) ) ]
		[ DataRow( TestData.InputData_EntryNotClosed ) ]
		public void ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InvalidInputFieldException) ) ]
		[ DataRow( TestData.InputData_TagFieldContainsSpaces ) ]
		public void ParseEntriesFromDataTest_ThrowsInvalidInputFieldException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( MissingInputFieldException) ) ]
		[ DataRow( TestData.InputData_MissingIdentityField ) ]
		[ DataRow( TestData.InputData_MissingNameField ) ]
		[ DataRow( TestData.InputData_MissingTagField ) ]
		public void ParseEntriesFromDataTest_ThrowsMissingInputFieldException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( UnexpectedCharacterException) ) ]
		[ DataRow( TestData.InputData_UnexpectedCharBetweenEntries ) ]
		[ DataRow( TestData.InputData_UnexpectedCharInEntry ) ]
		public void ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException( string data )
		{
			EntryParser.ParseEntriesFromData( SplitDataString( data ), ref i );
		}


		private static string[] SplitDataString( string data )
		{
			Log.Info( $"raw data: \"{ data }\"" );
			var tokens = data.Split( '|' );
			Log.Info( "tokenized data:" );
			foreach( var token in tokens )
			{
				Log.Info( $"	\"{ token }\"" );
			}
			Log.Info();
			return tokens;
		}
	}
}