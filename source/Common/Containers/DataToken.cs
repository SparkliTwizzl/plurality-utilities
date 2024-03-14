namespace Petrichor.Common.Containers
{
	public struct DataToken< T > where T : new()
	{
		public int TokenLinesLength { get; set; } = 0;
		public T Value { get; set; } = new();

		public DataToken() { }
	}
}
