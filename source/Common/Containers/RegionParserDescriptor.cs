namespace Petrichor.Common.Containers
{
	public struct RegionParserDescriptor<T> where T : new()
	{
		public Dictionary<string, int> MaxAllowedTokenInstances { get; set; } = new();
		public Dictionary<string, int> MinRequiredTokenInstances { get; set; } = new();
		public Func<T, T> PostParseHandler { get; set; } = ( T result ) => result;
		public Func<T> PreParseHandler { get; set; } = () => new T();
		public string RegionName { get; set; } = string.Empty;
		public Dictionary<string, Func<IndexedString[], int, T, RegionData<T>>> TokenHandlers { get; set; } = new();


		public RegionParserDescriptor() { }
	}
}
