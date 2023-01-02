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


		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
		}


		[TestMethod]
		public void ParseFileTest_Success()
		{
			parser.ParseFile(LocateInputFile("TestInput_Valid"));
			var expected = new Person[]
			{
				new Person()
				{
					Identities =
					{
						new Identity()
						{
							Name = "Name1",
							Tag = "n1",
						},
						new Identity()
						{
							Name = "Nickname1",
							Tag = "k1",
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
							Tag = "n2",
						}
					},
					Pronoun = "pronouns2",
					Decoration = "decoration2",
				},
			};
			var actual = parser.People.ToArray();
			CollectionAssert.AreEqual(expected, actual);
		}

		// throws BlankFieldException if file contains a field with no value
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(BlankFieldException))]
		[DataRow()]
		public void ParseFileTest_ThrowsBlankFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateFieldException))]
		[DataRow("TestInput_TooManyDecorations.akf")]
		[DataRow("TestInput_TooManyPronouns.akf")]
		public void ParseFileTest_ThrowsDuplicatedFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		// throws EntryNotClosedException if file contains an entry that is not closed
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(EntryNotClosedException))]
		[DataRow()]
		public void ParseFileTest_ThrowsEntryNotClosedException(string fileName)
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

		// throws InvalidFieldException if file contains a field that could not be parsed correctly
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(InvalidFieldException))]
		[DataRow()]
		public void ParseFileTest_ThrowsInvalidFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		// throws MissingFieldException if file contains a an entry with no name fields
		// throws MissingFieldException if file contains a name field with no paired tag field
		// throws MissingFieldException if file contains a tag field with no paired name field
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(MissingFieldException))]
		[DataRow()]
		public void ParseFileTest_ThrowsMissingFieldException(string fileName)
		{
			parser.ParseFile(LocateInputFile(fileName));
		}

		// throws UnexpectedCharacterException if file contains a line that starts with an unexpected character
		//TODO create input files
		//TODO write data rows
		[TestMethod]
		[ExpectedException(typeof(UnexpectedCharacterException))]
		[DataRow()]
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
