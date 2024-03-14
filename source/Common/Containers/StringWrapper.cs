namespace Petrichor.Common.Containers
{
	public struct StringWrapper
	{
		public string Value { get; set; } = string.Empty;

		public StringWrapper() { }
		public StringWrapper( string value ) => Value = value;
		public StringWrapper( StringWrapper other ) => Value = other.Value;
		public static implicit operator string( StringWrapper p ) => new( p.Value );
	}
}
