namespace Petrichor.Common.Containers
{
	public sealed class ProcessedRegionData<T> : IEquatable<ProcessedRegionData<T>> where T : class, new()
	{
		public int BodySize { get; set; } = 0;
		public T Value { get; set; } = new();


		public ProcessedRegionData() { }
		public ProcessedRegionData( T value, int bodySize = 0 )
		{
			BodySize = bodySize;
			Value = value;
		}
		public ProcessedRegionData( ProcessedRegionData<T> other )
		{
			BodySize = other.BodySize;
			Value = other.Value;
		}


		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}
			var other = obj as ProcessedRegionData<T>;
			return Equals( other! );
		}

		public bool Equals( ProcessedRegionData<T>? other )
		{
			if ( other is null )
			{
				return false;
			}
			var bodySizeEqual = BodySize.Equals( other.BodySize );
			var valueEqual = EqualityComparer<T>.Default.Equals( Value, other.Value );
			return bodySizeEqual && valueEqual;
		}

		public override int GetHashCode() => BodySize.GetHashCode() ^ Value!.GetHashCode();

		public override string ToString() => $"{BodySize} {Value!}";

		public static bool operator ==( ProcessedRegionData<T> a, ProcessedRegionData<T> b ) => a.Equals( b );

		public static bool operator !=( ProcessedRegionData<T> a, ProcessedRegionData<T> b ) => !a.Equals( b );
	}
}
