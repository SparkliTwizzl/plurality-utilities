namespace Petrichor.Common.Utilities
{
	public interface IRegionParser
	{
		bool HasParsedMaxAllowedRegions { get; }
		int MaxRegionsAllowed { get; }
		int RegionsParsed {  get; }

		
		T Parse<T>( string[] regionData, ref int i );
	}
}
