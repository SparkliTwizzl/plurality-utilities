using PluralityUtilities.AutoHotkeyScripts.Containers;


namespace PluralityUtilities.TestCommon.TestData
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
	}
}
