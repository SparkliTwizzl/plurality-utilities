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
			RawLine = line.ToString();
			GetTokenDataFromLine( RawLine );
		}


		private void GetTokenDataFromLine( string rawLine )
		{
			var line = rawLine.Trim();
			var lineCommentTokenIndex = line.IndexOf( Tokens.LineComment.Key );
			var doesTokenContainLineComment = lineCommentTokenIndex >= 0;
			if ( doesTokenContainLineComment )
			{
				line = line[ ..lineCommentTokenIndex ];
			}

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
	}
}
