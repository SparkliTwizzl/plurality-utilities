namespace Petrichor.Common.Utilities
{
	public interface IRegionParser<out T>
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		int RegionsParsed {  get; }

		
		T Parse( string[] regionData );
	}
}
