using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntryRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		static int MaxRegionsAllowed { get; }
		static string RegionName { get; } = string.Empty;
		int RegionsParsed { get; }


		ScriptEntry Parse( string[] regionData );
	}
}
