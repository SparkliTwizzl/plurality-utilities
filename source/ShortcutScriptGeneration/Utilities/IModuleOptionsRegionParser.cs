using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IModuleOptionsRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		static string RegionName { get; } = string.Empty;
		int RegionsParsed { get; }


		ScriptModuleOptions Parse( string[] regionData );
	}
}
