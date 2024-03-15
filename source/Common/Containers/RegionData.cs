namespace Petrichor.Common.Containers
{
	public struct RegionData< T > where T : new()
	{
		public int BodySize { get; set; } = 0;
		public T Value { get; set; } = new();


		public RegionData() { }
	}
}
