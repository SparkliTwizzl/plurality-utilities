namespace Petrichor.Common.Containers
{
	public struct RegionParserDescriptor< T >  where T : new()
	{
		public int MaxRegionsAllowed { get; set; } = 0;
		public int MinRegionsRequired { get; set; } = 0;
		public Func< T, T > PostParseHandler { get; set; } = ( T result ) => new T();
		public Func< T > PreParseHandler { get; set; } = () => new T();
		public string RegionName { get; set; } = string.Empty;
		public Dictionary< string, Func< string[], int, T, RegionData< T > > > TokenHandlers { get; set; } = new();


		public RegionParserDescriptor() { }
	}
}
