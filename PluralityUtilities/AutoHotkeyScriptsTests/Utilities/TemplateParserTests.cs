using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class TemplateParserTests
	{
		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
		}


		[TestMethod]
		[DataRow("#-@-$-&", "Alex-ax-they/them-a person")]
		public void ParseMacroFromTemplateTest_Success(string template, string expected)
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
				Decoration = "a person",
			};
			var actual = TemplateParser.CreateMacroFromTemplate(template, person.Identities[0], person.Pronoun, person.Decoration);
			Assert.AreEqual(expected, actual);
		}
	}
}