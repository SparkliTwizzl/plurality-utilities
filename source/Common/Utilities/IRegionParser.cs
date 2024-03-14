using Petrichor.Common.Containers;


namespace Petrichor.Common.Utilities
{
	public interface IRegionParser< T > where T : new()
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		string RegionName { get; }
		int RegionsParsed { get; }


		DataToken< T > Parse( string[] regionData );
	}
}
