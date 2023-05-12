using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class InputParserTests
	{
		[ TestInitialize ]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		[ DataRow( "InputParser_Valid.txt" ) ]
		public void ParseFileTest_Success( string fileName )
		{
			var expected = ExpectedOutputData.ParsedInputData;
			var actual = InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
			CollectionAssert.AreEqual( expected, actual );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( BlankInputFieldException) ) ]
		[ DataRow( "InputParser_BlankDecorationField.txt" ) ]
		[ DataRow( "InputParser_BlankNameField.txt" ) ]
		[ DataRow( "InputParser_BlankPronounField.txt" ) ]
		[ DataRow( "InputParser_BlankTagField.txt" ) ]
		public void ParseFileTest_ThrowsBlankInputFieldException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( DuplicateInputFieldException) ) ]
		[ DataRow( "InputParser_TooManyDecorationFields.txt" ) ]
		[ DataRow( "InputParser_TooManyPronounFields.txt" ) ]
		public void ParseFileTest_ThrowsDuplicateInputFieldException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( FileNotFoundException) ) ]
		[ DataRow( "nonexistent.txt" ) ]
		public void ParseFileTest_ThrowsFileNotFoundException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InputEntryNotClosedException) ) ]
		[ DataRow( "InputParser_EntryNotClosed.txt" ) ]
		public void ParseFileTest_ThrowsInputEntryNotClosedException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( InvalidInputFieldException) ) ]
		[ DataRow( "InputParser_TagFieldContainsSpaces.txt" ) ]
		public void ParseFileTest_ThrowsInvalidInputFieldException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( MissingInputFieldException) ) ]
		[ DataRow( "InputParser_MissingIdentityField.txt" ) ]
		[ DataRow( "InputParser_MissingNameField.txt" ) ]
		[ DataRow( "InputParser_MissingTagField.txt" ) ]
		public void ParseFileTest_ThrowsMissingInputFieldException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}

		[ TestMethod ]
		[ ExpectedException( typeof( UnexpectedCharacterException) ) ]
		[ DataRow( "InputParser_UnexpectedCharacterBetweenEntries.txt" ) ]
		[ DataRow( "InputParser_UnexpectedCharacterInsideEntry.txt" ) ]
		public void ParseFileTest_ThrowsUnexpectedCharacterException( string fileName )
		{
			InputParser.ParsePeopleFromFile( TestUtilities.LocateInputFile( fileName ) );
		}
	}
}
