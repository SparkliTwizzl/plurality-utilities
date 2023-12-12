namespace PluralityUtilities.AutoHotkeyScripts.LookUpTables
{
	public static class TemplateMarkers
	{
		public static readonly Dictionary< char, string > LookUpTable = new Dictionary< char, string >()
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
