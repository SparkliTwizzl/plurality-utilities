namespace Petrichor.Common.Utilities
{
	public interface IRegionParser< out T > where T : class, new()
	{
		int LinesParsed { get; }
		Dictionary< string, int > MaxAllowedTokenInstances { get; }
		Dictionary< string, int > MinRequiredTokenInstances { get; }
		string RegionName { get; }
		Dictionary< string, int > TokenInstancesParsed { get; }


		T Parse( string[] regionData );
	}
}
