using Petrichor.Common.Syntax;


namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Represents a tokenized string.
	/// </summary>
	public class StringToken
	{
		/// <summary>
		/// The line number from which this token was extracted.
		/// </summary>
		public int LineNumber { get; set; } = 0;

		/// <summary>
		/// The key extracted from the tokenized string.
		/// </summary>
		public string TokenKey { get; set; } = string.Empty;

		/// <summary>
		/// The raw, unprocessed line from which the token was derived.
		/// </summary>
		public string OriginalLine { get; set; } = string.Empty;

		/// <summary>
		/// The value associated with the token, if any.
		/// </summary>
		public string TokenValue { get; set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="StringToken"/> class.
		/// </summary>
		public StringToken() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="StringToken"/> class by copying another instance.
		/// </summary>
		/// <param name="other">The instance of <see cref="StringToken"/> to copy.</param>
		public StringToken(StringToken other)
		{
			LineNumber = other.LineNumber;
			TokenKey = other.TokenKey;
			OriginalLine = other.OriginalLine;
			TokenValue = other.TokenValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringToken"/> class from a line of input text.
		/// </summary>
		/// <param name="line">The line of input text to tokenize.</param>
		public StringToken(string line)
		{
			OriginalLine = line;
			ParseTokenFromLine(OriginalLine);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringToken"/> class from an indexed string.
		/// </summary>
		/// <param name="line">The indexed string to tokenize.</param>
		public StringToken(IndexedString line)
		{
			LineNumber = line.LineNumber;
			OriginalLine = line.Value;
			ParseTokenFromLine(OriginalLine);
		}


		/// <summary>
		/// Extracts the key and value from the given raw line.
		/// </summary>
		/// <param name="rawLine">The raw line to parse for token data.</param>
		private void ParseTokenFromLine(string rawLine)
		{
			var trimmedLine = RemoveCommentFromLine(rawLine.Trim());
			var keyDelimiterIndex = trimmedLine.IndexOf(ControlSequences.TokenKeyDelimiter);
			var hasValue = keyDelimiterIndex >= 0;

			if (!hasValue)
			{
				TokenKey = trimmedLine.Trim();
				return;
			}

			TokenKey = trimmedLine[..keyDelimiterIndex].Trim();
			var valueStartIndex = keyDelimiterIndex + 1;
			TokenValue = trimmedLine[valueStartIndex..].Trim();
		}

		/// <summary>
		/// Removes any line comments from the given line.
		/// </summary>
		/// <param name="line">The line to process for comments.</param>
		/// <returns>The line without comments.</returns>
		private static string RemoveCommentFromLine(string line)
		{
			var commentStartIndex = line.IndexOf(TokenPrototypes.LineComment.Key);
			var hasComment = commentStartIndex >= 0;

			if (!hasComment)
			{
				return line;
			}

			var escapeSequenceLength = ControlSequences.Escape.ToString().Length;
			var isCommentEscaped = (commentStartIndex >= escapeSequenceLength)
				&& (line[commentStartIndex - escapeSequenceLength] == ControlSequences.Escape);

			if (!isCommentEscaped)
			{
				return line[..commentStartIndex];
			}

			var commentEndIndex = commentStartIndex - escapeSequenceLength;
			var lineBeforeComment = line[..commentEndIndex];
			var lineAfterCommentStartIndex = commentStartIndex + ControlSequences.LineComment.Length;
			var lineAfterComment = RemoveCommentFromLine(line[lineAfterCommentStartIndex..]);
			return $"{lineBeforeComment}{ControlSequences.LineComment}{lineAfterComment}".Trim();
		}
	}
}
