namespace Petrichor.RandomStringGeneration.Containers
{
	public sealed class InputData : IEquatable<InputData>
	{
		public string AllowedCharacters { get; set; } = string.Empty;
		public int StringCount { get; set; } = 0;
		public int StringLength { get; set; } = 0;


		public InputData() { }
		public InputData( string allowedCharacters, int stringCount, int stringLength )
		{
			AllowedCharacters = allowedCharacters;
			StringCount = stringCount;
			StringLength = stringLength;
		}
		public InputData( InputData other )
		{
			AllowedCharacters = other.AllowedCharacters;
			StringCount = other.StringCount;
			StringLength = other.StringLength;
		}


		public static bool operator ==( InputData a, InputData b ) => a.Equals( b );

		public static bool operator !=( InputData a, InputData b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( InputData ) obj );
		}

		public bool Equals( InputData? other )
		{
			if ( other is null )
			{
				return false;
			}
			return AllowedCharacters.Equals( other.AllowedCharacters ) && StringCount.Equals( other.StringCount ) && StringLength.Equals( other.StringLength );
		}

		public override int GetHashCode() => AllowedCharacters.GetHashCode() ^ StringCount.GetHashCode() ^ StringLength.GetHashCode();
	}
}
