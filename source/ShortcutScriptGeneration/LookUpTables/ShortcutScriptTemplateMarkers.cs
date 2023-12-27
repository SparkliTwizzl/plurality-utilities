namespace Petrichor.ShortcutScriptGeneration.LookUpTables
{
	public static class ShortcutScriptTemplateMarkers
	{
		public static Dictionary<char, string> LookUpTable => new()
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
