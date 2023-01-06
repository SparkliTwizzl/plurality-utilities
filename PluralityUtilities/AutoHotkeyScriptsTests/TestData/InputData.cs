using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Tests.TestData;


namespace PluralityUtilities.AutoHotkeyScriptsTests.TestData
{
	public static class InputData
	{
		public static class AutoHotkeyScriptGeneratorData
		{
			public static readonly string[] ValidMacroTemplates = ExpectedOutputData.CreatedMacroData;
		}

		public static class TemplateParserData
		{
			public static readonly Person[] ValidPeople = new Person[]
			{
				new Person()
				{
					Identities = new List<Identity>()
					{
						new Identity()
						{
							Name = "Sam",
							Tag = "sm",
						},
						new Identity()
						{
							Name = "Sammy",
							Tag = "smy",
						},
					},
					Pronoun = "they/them",
					Decoration = "-> a person",
				},
				new Person()
				{
					Identities = new List<Identity>()
					{
						new Identity()
						{
							Name = "Alex",
							Tag = "ax",
						},
					},
				}
			};
			public static readonly string[] ValidTemplates = ExpectedOutputData.ParsedTemplateData;
		}
}
}
