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
		[ExpectedException(typeof(InvalidArgumentException))]
		public void ParseFileTest_InvalidInputFileExtension()
		{
			parser.ParseFile("invalid.extension");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ParseFileTest_FileDoesNotExist()
		{
			parser.ParseFile(LocateInputFile("nonexistent"));
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


		private string LocateInputFile(string inputFile)
		{
			return $"{TestDirectories.TestInputDir}{inputFile}.akf";
		}
	}
}