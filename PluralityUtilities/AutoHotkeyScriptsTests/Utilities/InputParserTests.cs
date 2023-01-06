using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class InputParserTests
	{
		public InputParser parser = new InputParser();


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[TestMethod]
		[DataRow("InputParser_Valid.akf")]
		public void ParseFileTest_Success(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
			var expected = ValidData.ExpectedParsedInputData;
			var actual = parser.People.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(BlankInputFieldException))]
		[DataRow("InputParser_BlankDecorationField.akf")]
		[DataRow("InputParser_BlankNameField.akf")]
		[DataRow("InputParser_BlankPronounField.akf")]
		[DataRow("InputParser_BlankTagField.akf")]
		public void ParseFileTest_ThrowsBlankInputFieldException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateInputFieldException))]
		[DataRow("InputParser_TooManyDecorationFields.akf")]
		[DataRow("InputParser_TooManyPronounFields.akf")]
		public void ParseFileTest_ThrowsDuplicateInputFieldException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InputEntryNotClosedException))]
		[DataRow("InputParser_EntryNotClosed.akf")]
		public void ParseFileTest_ThrowsInputEntryNotClosedException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		[DataRow("nonexistent.akf")]
		public void ParseFileTest_ThrowsFileNotFoundException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidArgumentException))]
		[DataRow("invalid.extension")]
		public void ParseFileTest_ThrowsInvalidArgumentExtension(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidInputFieldException))]
		[DataRow("InputParser_TagFieldContainsSpaces.akf")]
		public void ParseFileTest_ThrowsInvalidInputFieldException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(MissingInputFieldException))]
		[DataRow("InputParser_MissingIdentityField.akf")]
		[DataRow("InputParser_MissingNameField.akf")]
		[DataRow("InputParser_MissingTagField.akf")]
		public void ParseFileTest_ThrowsMissingInputFieldException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(UnexpectedCharacterException))]
		[DataRow("InputParser_UnexpectedCharacterBetweenEntries.akf")]
		[DataRow("InputParser_UnexpectedCharacterInsideEntry.akf")]
		public void ParseFileTest_ThrowsUnexpectedCharacterException(string fileName)
		{
			parser.ParseFile(TestUtilities.LocateInputFile(fileName));
		}
	}
}
