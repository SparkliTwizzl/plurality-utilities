namespace Petrichor.Common.Containers
{
	public sealed class IndexedString : IEquatable<IndexedString>
	{
		public static IndexedString Empty => new();


		public string Value { get; set; } = string.Empty;
		public int LineNumber { get; set; } = 0;


		public IndexedString() { }
		public IndexedString( IndexedString other )
		{
			Value = other.Value;
			LineNumber = other.LineNumber;
		}
		public IndexedString( string value ) => Value = value;
		public IndexedString( string value, int lineNumber )
		{
			Value = value;
			LineNumber = lineNumber;
		}


		public static IndexedString[] IndexStringArray( string[] strings )
		{
			var result = new List<IndexedString>();
			for ( var i = 0 ; i < strings.Length ; ++i )
			{
				result.Add( new( strings[ i ], i + 1 ) );
			}
			return result.ToArray();
		}


		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}

			var other = obj as IndexedString;
			return Equals( other );
		}

		public bool Equals( IndexedString? other )
		{
			if ( other is null )
			{
				return false;
			}

			return Value.Equals( other.Value ) && LineNumber.Equals( other.LineNumber );
		}

		public override int GetHashCode() => Value.GetHashCode() ^ LineNumber.GetHashCode();

		public override string ToString() => $"<{LineNumber}> {Value}";

		public static bool operator ==( IndexedString a, IndexedString b ) => a.Equals( b );

		public static bool operator !=( IndexedString a, IndexedString b ) => !a.Equals( b );
	}
}
