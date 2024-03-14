using Petrichor.Common.Containers;


namespace Petrichor.Common.Utilities
{
	public interface IRegionParser< T > where T : class, new()
	{
		bool HasParsedMaxAllowedRegions { get; }
		int LinesParsed { get; }
		int MaxRegionsAllowed { get; }
		string RegionName { get; }
		int RegionsParsed { get; }


		T Parse( string[] regionData );

		void SetTokenHandlers( Dictionary< string, Action< StringToken, T >> tokenHandlers );
	}
}
