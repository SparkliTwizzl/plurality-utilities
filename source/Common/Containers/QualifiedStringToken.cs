using Petrichor.Common.Enums;
using Petrichor.Common.Info;


namespace Petrichor.Common.Containers
{
	public class QualifiedStringToken
	{
		public StringTokenQualifiers Qualifier { get; set; } = StringTokenQualifiers.Unknown;
		public string Value { get; set; } = string.Empty;


		public QualifiedStringToken() { }

		public QualifiedStringToken( QualifiedStringToken other )
		{
			Qualifier = other.Qualifier;
			Value = other.Value;
		}

		public QualifiedStringToken( string token ) => Value = TrimToken( token.Trim() );


		private string TrimToken( string token )
		{
			if ( token.IndexOf( CommonSyntax.LineCommentToken ) == 0 )
			{
				return string.Empty;
			}

			else if ( token.Contains( CommonSyntax.LineCommentToken ) )
			{
				var commentStart = token.IndexOf( CommonSyntax.LineCommentToken );
				return token[ 0..commentStart ].Trim();
			}

			return token.Trim();
		}
	}
}
