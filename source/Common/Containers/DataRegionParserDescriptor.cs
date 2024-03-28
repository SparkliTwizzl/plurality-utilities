namespace Petrichor.Common.Containers
{
	public struct DataRegionParserDescriptor<T> where T : class, new()
	{
		public Func<T, T> PostParseHandler { get; set; } = ( T result ) => result;
		public Func<T> PreParseHandler { get; set; } = () => new T();
		public DataToken RegionToken { get; set; } = new();
		public Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedRegionData<T>>> TokenHandlers { get; set; } = new();


		public DataRegionParserDescriptor() { }
	}
}
