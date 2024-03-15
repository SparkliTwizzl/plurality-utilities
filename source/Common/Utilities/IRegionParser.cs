namespace Petrichor.Common.Utilities
{
	public interface IRegionParser< out T > where T : class, new()
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		string RegionName { get; }
		int RegionsParsed { get; }


		T Parse( string[] regionData );
	}
}
