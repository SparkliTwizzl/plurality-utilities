using Petrichor.Common.Syntax;

namespace Petrichor.Common.Containers
{
	public class StringToken
	{
		public int LineNumber { get; set; } = 0;
		public string Key { get; set; } = string.Empty;
		public string RawLine { get; set; } = string.Empty;
		public string Value { get; set; } = string.Empty;


		public StringToken() { }
		public StringToken( StringToken other )
		{
			LineNumber = other.LineNumber;
			Key = other.Key;
			RawLine = other.RawLine;
			Value = other.Value;
		}
		public StringToken( string line )
		{
			RawLine = line;
			GetTokenDataFromLine( RawLine );
		}
		public StringToken( IndexedString line )
		{
			LineNumber = line.LineNumber;
			RawLine = line.Value;
			GetTokenDataFromLine( RawLine );
		}


		private void GetTokenDataFromLine( string rawLine )
		{
			var line = TrimLineCommentFromLine( rawLine.Trim() );
			var nameEndsAt = line.IndexOf( ':' );
			var doesTokenContainAValue = nameEndsAt >= 0;
			if ( !doesTokenContainAValue )
			{
				Key = line.Trim();
				return;
			}
			Key = line[ ..nameEndsAt ].Trim();
			var valueStartsAt = nameEndsAt + 1;
			Value = line[ valueStartsAt.. ].Trim();
		}

		private static string TrimLineCommentFromLine( string line )
		{
			var lineCommentIndex = line.IndexOf( Tokens.LineComment.Key );
			var doesContainLineComment = lineCommentIndex >= 0;
			if ( !doesContainLineComment )
			{
				return line;
			}

			var escapeSequenceLength = ControlSequences.Escape.ToString().Length;
			var isLineCommentEscaped = ( lineCommentIndex >= escapeSequenceLength )
				&& ( line[ lineCommentIndex - escapeSequenceLength ] == ControlSequences.Escape );

			if ( !isLineCommentEscaped )
			{
				return line[ ..lineCommentIndex ];
			}

			var firstPartEndIndex = lineCommentIndex - escapeSequenceLength;
			var firstPart = line[ ..firstPartEndIndex ];
			var secondPartStartIndex = lineCommentIndex + ControlSequences.LineComment.Length;
			var secondPart = TrimLineCommentFromLine( line[ secondPartStartIndex.. ] );
			var processedLine = $"{firstPart}{ControlSequences.LineComment}{secondPart}";
			return processedLine.Trim();
		}
	}
}
