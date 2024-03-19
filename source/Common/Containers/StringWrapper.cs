namespace Petrichor.Common.Containers
{
	public sealed class StringWrapper : IEquatable<StringWrapper>
	{
		public string Value { get; set; } = string.Empty;


		public StringWrapper() { }
		public StringWrapper( StringWrapper other ) => Value = other.Value;
		public StringWrapper( string value ) => Value = value;


		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}

			var other = obj as StringWrapper;
			return Equals( other );
		}

		public bool Equals( StringWrapper? other )
		{
			if ( other is null )
			{
				return false;
			}

			return Value.Equals( other.Value );
		}

		public override int GetHashCode() => Value.GetHashCode();

		public override string ToString() => Value;

		public static bool operator ==( StringWrapper a, StringWrapper b ) => a.Equals( b );

		public static bool operator !=( StringWrapper a, StringWrapper b ) => !a.Equals( b );
	}
}
