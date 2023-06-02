using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.TestCommon.Utilities;

namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[ TestClass ]
	public class EntryParserTests
	{
		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[ TestMethod ]
		public void ParseEntriesFromDataTest_()
		{
			Assert.Fail();
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