namespace Petrichor.Common.Containers
{
	public struct RegionParserDescriptor< T >  where T : new()
	{
		public string RegionName { get; set; } = string.Empty;
		public int MaxRegionsAllowed { get; set; } = 0;
		public int MinRegionsRequired { get; set; } = 0;
		public Dictionary< string, Func< string[], int, T, RegionData< T > > > TokenHandlers { get; set; } = new();


		public RegionParserDescriptor() { }
	}
}
