using PluralityUtilities.AutoHotkeyScripts.Containers;


namespace PluralityUtilities.AutoHotkeyScripts.Tests.TestData
{
	public static class ValidData
	{
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

		public static string[] expectedValidOutputData = new string[]
		{
			"::tag1;::Name1; ",
			"::tag1-;::Name1 (pronouns1); ",
			"::tag1--::Name1 (pronouns1)",
			"::tag1-/::Name1 (pronouns1) decoration1",
			"",
			"::tag1a;::Nickname1; ",
			"::tag1a-;::Nickname1 (pronouns1); ",
			"::tag1a--::Nickname1 (pronouns1)",
			"::tag1a-/::Nickname1 (pronouns1) decoration1",
			"",
			"::tag2;::Name2; ",
			"::tag2-;::Name2 (); ",
			"::tag2--::Name2 ()",
			"::tag2-/::Name2 () ",
			"",
		};
	}
}
