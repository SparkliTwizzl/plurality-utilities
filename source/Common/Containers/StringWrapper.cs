namespace Petrichor.Common.Containers
{
	public class StringWrapper
	{
		public string Value {  get; set; } = string.Empty;
		public StringWrapper() { }
		public StringWrapper( StringWrapper other ) => Value = other.Value;
		public StringWrapper( string value ) => Value = value;
		public override string ToString() => Value;
	}
}
