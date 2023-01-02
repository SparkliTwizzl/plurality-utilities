using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class AkfFileParserTests
	{
		public AkfFileParser parser = new AkfFileParser();
		public static Person[] expectedValidInputData = new Person[]
			{
				new Person()
				{
					Identities =
					{
						new Identity()
						{
							Name = "Name1",
							Tag = "tag1",
						},
						new Identity()
						{
							Name = "Nickname1",
							Tag = "tag1a",
						},
					},
					Pronoun = "pronouns1",
					Decoration = "decoration1",
				},
				new Person()
				{
					Identities =
					{
						new Identity()
						{
							Name = "Name2",
							Tag = "tag2",
						}
					},
				},
			};


		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
		}


		[TestMethod]
		[DataRow("TestInput_Valid.akf")]
		public void ParseFileTest_Success(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
			var expected = expectedValidInputData;
			var actual = parser.People.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(BlankInputFieldException))]
		[DataRow("TestInput_BlankDecorationField.akf")]
		[DataRow("TestInput_BlankNameField.akf")]
		[DataRow("TestInput_BlankPronounField.akf")]
		[DataRow("TestInput_BlankTagField.akf")]
		public void ParseFileTest_ThrowsBlankInputFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateInputFieldException))]
		[DataRow("TestInput_TooManyDecorationFields.akf")]
		[DataRow("TestInput_TooManyPronounFields.akf")]
		public void ParseFileTest_ThrowsDuplicateInputFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InputEntryNotClosedException))]
		[DataRow("TestInput_EntryNotClosed.akf")]
		public void ParseFileTest_ThrowsInputEntryNotClosedException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		[DataRow("nonexistent")]
		public void ParseFileTest_ThrowsFileNotFoundException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidArgumentException))]
		[DataRow("invalid.extension")]
		public void ParseFileTest_ThrowsInvalidArgumentExtension(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidInputFieldException))]
		[DataRow("TestInput_TagFieldContainsSpaces.akf")]
		public void ParseFileTest_ThrowsInvalidInputFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		// throws MissingInputFieldException if file contains a an entry with no name fields
		// throws MissingInputFieldException if file contains a name field with no paired tag field
		// throws MissingInputFieldException if file contains a tag field with no paired name field
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(MissingInputFieldException))]
		[DataRow()]
		public void ParseFileTest_ThrowsMissingInputFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(UnexpectedCharacterException))]
		[DataRow("TestInput_UnexpectedCharacterBetweenEntries.akf")]
		[DataRow("TestInput_UnexpectedCharacterInsideEntry.akf")]
		public void ParseFileTest_ThrowsUnexpectedCharacterException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}


		private string LocateInputFile(string fileName)
		{
			return $"{TestDirectories.TestInputDir}{fileName}";
		}
	}
}
