using Petrichor.Common.Info;

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

		public StringToken( string rawToken ) => SplitAndStoreRawToken( rawToken.Trim() );


		private void SplitAndStoreRawToken( string rawToken )
		{
			var lineCommentTokenIndex = rawToken.IndexOf( CommonSyntax.LineCommentToken );
			var doesTokenContainLineComment = lineCommentTokenIndex > -1;
			if ( doesTokenContainLineComment )
			{
				rawToken = rawToken[ ..lineCommentTokenIndex ];
			}

			var nameEndsAt = rawToken.IndexOf( ':' );

			var doesTokenContainNoValue = nameEndsAt < 0;
			if ( doesTokenContainNoValue )
			{
				Name = rawToken.Trim();
				return;
			}

			Name = rawToken[ 0..nameEndsAt ].Trim();

			var valueStartsAt = nameEndsAt + 1;
			Value = rawToken[ valueStartsAt.. ].Trim();
		}
	}
}
