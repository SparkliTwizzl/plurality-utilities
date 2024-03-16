namespace Petrichor.Common.Utilities
{
	public interface IRegionParser< out T > where T : class, new()
	{
		bool HasParsedMaxAllowedRegions { get; }
		bool HasParsedMinRequiredRegions { get; }
		int LinesParsed { get; }
		Dictionary< string, int > MaxAllowedTokenInstances { get; }
		int MaxRegionsAllowed { get; }
		int MinRegionsRequired { get; }
		Dictionary< string, int > MinRequiredTokenInstances { get; }
		string RegionName { get; }
		int RegionsParsed { get; }
		Dictionary< string, int > TokenInstancesParsed { get; }


		T Parse( string[] regionData );
	}
}
