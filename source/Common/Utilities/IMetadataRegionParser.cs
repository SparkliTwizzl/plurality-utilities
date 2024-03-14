namespace Petrichor.Common.Utilities
{
	public interface IMetadataRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		static int MaxRegionsAllowed { get; }
		static string RegionName { get; } = string.Empty;
		int RegionsParsed { get; }
		static string RegionIsValidMessage { get; } = string.Empty;

		string Parse( string[] regionData );
	}
}
