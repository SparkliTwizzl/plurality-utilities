namespace PluralityUtilities.AutoHotkeyScripts.LookUpTables
{
	public static class TemplateMarkers
	{
		public static readonly Dictionary<char, string> dictionary = new Dictionary<char, string>()
		{
			{
				'#',
				"name"
			},
			{
				'@',
				"tag"
			},
			{
				'$',
				"pronoun"
			},
			{
				'&',
				"decoration"
			},
		};
	}
}
