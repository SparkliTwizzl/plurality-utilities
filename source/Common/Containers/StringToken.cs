namespace Petrichor.Common.Containers
{
	public class StringToken
	{
		public string Name { get; set; } = string.Empty;
		public string Value { get; set; } = string.Empty;


		public StringToken() { }

		public StringToken( StringToken other )
		{
			Name = other.Name;
			Value = other.Value;
		}

		public StringToken( string rawToken )
		{
			SplitAndStoreRawToken( rawToken );
		}

		public StringToken( QualifiedStringToken qualifiedToken )
		{
			SplitAndStoreRawToken( qualifiedToken.Value );
		}


		private void SplitAndStoreRawToken( string rawToken )
		{
			var nameEndsAt = rawToken.IndexOf( ':' );
			if ( nameEndsAt < 0 )
			{
				Name = rawToken.Trim();
				return;
			}

			var valueStartsAt = nameEndsAt + 1;
			var valueLength = rawToken.Length - valueStartsAt;

			Name = rawToken.Substring( 0, nameEndsAt ).Trim();
			Value = rawToken.Substring( valueStartsAt, valueLength );
		}
	}
}
