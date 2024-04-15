using Petrichor.Common.Syntax;
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

		public static string CodepointsToChars( this string input )
		{
			var regex = UnicodeCodepointRegex();
			var result = regex.Replace( input, match =>
			{
				var hexValue = match.Groups[ 1 ].Value;
				var codepoint = int.Parse( hexValue, System.Globalization.NumberStyles.HexNumber );
				return char.ConvertFromUtf32( codepoint );
			} );
			return result;
		}

		public static string EscapedCharsToCodepoints( this string input )
		{
			var regex = new Regex( $@"\{ControlSequences.Escape}." );
			var result = regex.Replace( input, match =>
			{
				var escaped = match.Captures[ 0 ].Value[ 1 ];
				var codepoint = char.ConvertToUtf32( escaped.ToString(), 0 );
				return $"U+{codepoint:X4}";
			} );
			return result;
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

		public static string TrimQuotes( this string input )
		{
			if ( input is null )
			{
				return string.Empty;
			}

			var startsWithQuote = input.StartsWith( '"' );
			var endsWithQuote = input.EndsWith( '"' );
			if ( startsWithQuote && endsWithQuote )
			{
				return input[ 1..( input.Length - 1 ) ];
			}
			if ( startsWithQuote )
			{
				return input[ 1.. ];
			}
			if ( endsWithQuote )
			{
				return input[ ..( input.Length - 1 ) ];
			}
			return input;
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


		[GeneratedRegex( "[a-zA-Z]" )]
		private static partial Regex AlphabeticRegex();

		private static bool IsAlphabetic( char c ) => AlphabeticRegex().Match( c.ToString() ).Success;

		[GeneratedRegex( "U\\+([0-9A-Fa-f]{4})" )]
		private static partial Regex UnicodeCodepointRegex();
	}
}
