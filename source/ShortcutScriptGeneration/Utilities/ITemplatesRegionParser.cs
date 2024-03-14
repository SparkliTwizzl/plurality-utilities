namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface ITemplatesRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		static string RegionName { get; } = string.Empty;
		int RegionsParsed { get; }
		int TemplatesParsed { get; }


		string[] Parse( string[] regionData );
	}
}
