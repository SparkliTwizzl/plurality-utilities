using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntriesRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		static string RegionName { get; } = string.Empty;
		int RegionsParsed { get; }


		ScriptEntry[] Parse( string[] regionData );
	}
}
