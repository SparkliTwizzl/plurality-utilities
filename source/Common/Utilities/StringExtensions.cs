using System.Text;
using System.Text.RegularExpressions;

namespace Petrichor.Common.Utilities
{
	public static partial class StringExtensions
	{
		public static string AddTrailingPathSeparator( this string input )
		{
			if ( input.EndsWith( Path.PathSeparator ) )
			{
				return input;
			}
			return $"{input}{Path.PathSeparator}";
		}

		public static string ToFirstCaps( this string input )
		{
			var isNewWord = true;
			var builder = new StringBuilder();
			foreach ( var c in input )
			{
				_ = builder.Append( isNewWord ? c.ToString().ToUpper() : c.ToString().ToLower() );
				isNewWord = !IsAlphabetic( c );
			}
			return builder.ToString();
		}

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


		[GeneratedRegex( "[a-z,A-Z]" )]
		private static partial Regex RegexAlphabetic();

		private static bool IsAlphabetic( char c ) => RegexAlphabetic().Match( c.ToString() ).Success;
	}
}
