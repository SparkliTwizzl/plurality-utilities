namespace Petrichor.AutoHotkeyScripts.LookUpTables
{
	public static class ShortcutScriptTemplateMarkers
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
