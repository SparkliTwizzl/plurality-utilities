using Petrichor.Common.Syntax;
using System.Text;
using System.Text.RegularExpressions;


namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Provides extension methods for string manipulations.
	/// </summary>
	public static partial class StringExtensions
	{
		/// <summary>
		/// Ensures the input string has a trailing path separator.
		/// </summary>
		/// <param name="input">The input string to modify.</param>
		/// <returns>The input string with a trailing path separator.</returns>
		public static string AddTrailingPathSeparator(this string input)
		{
			if (input.EndsWith(Path.PathSeparator))
			{
				return input;
			}
			return $"{input}{Path.PathSeparator}";
		}

		/// <summary>
		/// Converts Unicode codepoints in the input string to characters.
		/// </summary>
		/// <param name="input">The string containing Unicode codepoints.</param>
		/// <returns>A string where Unicode codepoints have been converted to characters.</returns>
		public static string CodepointsToChars(this string input)
		{
			var regex = UnicodeCodepointRegex();
			var result = regex.Replace(input, match =>
			{
				var hexValue = match.Groups[1].Value;
				var codepoint = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
				return char.ConvertFromUtf32(codepoint);
			});
			return result;
		}

		/// <summary>
		/// Converts escaped characters in the input string to Unicode codepoints.
		/// </summary>
		/// <param name="input">The string containing escaped characters.</param>
		/// <returns>A string where escaped characters have been converted to Unicode codepoints.</returns>
		public static string EscapedCharsToCodepoints(this string input)
		{
			var regex = new Regex($@"\{ControlSequences.Escape}.");
			var result = regex.Replace(input, match =>
			{
				var escaped = match.Captures[0].Value[1];
				var codepoint = char.ConvertToUtf32(escaped.ToString(), 0);
				return $"U+{codepoint:X4}";
			});
			return result;
		}

		/// <summary>
		/// Capitalizes the first letter of each word in the input string, converting the rest to lowercase.
		/// </summary>
		/// <param name="input">The string to modify.</param>
		/// <returns>The input string with the first letter of each word capitalized.</returns>
		public static string ToFirstCaps(this string input)
		{
			var isNewWord = true;
			var builder = new StringBuilder();
			foreach (var c in input)
			{
				_ = builder.Append(isNewWord ? c.ToString().ToUpper() : c.ToString().ToLower());
				isNewWord = !IsAlphabetic(c);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Trims leading and trailing double quotes from the input string.
		/// </summary>
		/// <param name="input">The string to modify.</param>
		/// <returns>The input string without leading or trailing quotes.</returns>
		public static string TrimQuotes(this string input)
		{
			if (input is null)
			{
				return string.Empty;
			}

			var startsWithQuote = input.StartsWith('"');
			var endsWithQuote = input.EndsWith('"');
			if (startsWithQuote && endsWithQuote)
			{
				return input[1..(input.Length - 1)];
			}
			if (startsWithQuote)
			{
				return input[1..];
			}
			if (endsWithQuote)
			{
				return input[..(input.Length - 1)];
			}
			return input;
		}

		/// <summary>
		/// Wraps the input string in double quotes.
		/// </summary>
		/// <param name="input">The string to wrap in quotes.</param>
		/// <returns>The input string wrapped in double quotes.</returns>
		public static string WrapInQuotes(this string input)
		{
			var startsWithQuote = input.StartsWith('"');
			var endsWithQuote = input.EndsWith('"');
			if (!startsWithQuote && !endsWithQuote)
			{
				return string.Format("\"{0}\"", input);
			}
			if (!startsWithQuote)
			{
				return string.Format("\"{0}", input);
			}
			if (!endsWithQuote)
			{
				return string.Format("{0}\"", input);
			}
			return input;
		}

		/// <summary>
		/// Regular expression to match alphabetic characters.
		/// </summary>
		/// <returns>The regex for alphabetic characters.</returns>
		[GeneratedRegex("[a-zA-Z]")]
		private static partial Regex AlphabeticRegex();

		/// <summary>
		/// Checks if a character is alphabetic.
		/// </summary>
		/// <param name="c">The character to check.</param>
		/// <returns>True if the character is alphabetic; otherwise, false.</returns>
		private static bool IsAlphabetic(char c) => AlphabeticRegex().Match(c.ToString()).Success;

		/// <summary>
		/// Regular expression to match Unicode codepoints in the form U+XXXX.
		/// </summary>
		/// <returns>The regex for Unicode codepoints.</returns>
		[GeneratedRegex("U\\+([0-9A-Fa-f]{4})")]
		private static partial Regex UnicodeCodepointRegex();
	}
}
