namespace Petrichor.Common.Utilities
{
	public interface IMetadataRegionParser : IRegionParser<string>
	{
		static string RegionIsValidMessage { get; } = string.Empty;

		new string Parse( string[] regionData );
	}
}
