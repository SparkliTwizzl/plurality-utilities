namespace Petrichor.Common.Utilities
{
	public static class StringExtensions
	{
		public static string WrapInQuotes( this string input )
		{
			var startsWithQuote = input.StartsWith( '"' );
			var endsWithQuote = input.EndsWith( '"' );
			if ( !startsWithQuote && !endsWithQuote )
			{
				return string.Format( "\"{0}\"", input );
			}
			if ( !startsWithQuote )
			{
				return string.Format( "\"{0}", input );
			}
			if ( !endsWithQuote )
			{
				return string.Format( "{0}\"", input );
			}
			return input;
		}
	}
}
