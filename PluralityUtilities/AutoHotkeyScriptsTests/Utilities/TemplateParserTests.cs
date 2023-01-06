using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.TestCommon.Utilities;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class TemplateParserTests
	{



		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
		}


		[TestMethod]
		[DataRow("<@> #", "<ax> Alex")]
		[DataRow("<@/> # ($) &", "<ax/> Alex (they/them) -> a person")]
		public void CreateMacroFromTemplateTest_Success(string template, string expected)
		{
			var person = new Person()
			{
				Identities = new List<Identity>()
				{
					new Identity()
					{
						Name = "Alex",
						Tag = "ax",
					},
				},
				Pronoun = "they/them",
				Decoration = "// a person",
			};
			var actual = TemplateParser.CreateMacroFromTemplate(template, person.Identities[0], person.Pronoun, person.Decoration);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[DataRow("TemplateParser_ValidTemplates.txt")]
		public void ParseTemplatesFromFileTest_Success(string inputFile)
		{
			var filePath = TestUtilities.LocateInputFile(inputFile);

			Assert.Fail();
		}
	}
}