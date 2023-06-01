using PluralityUtilities.Common.Enums;

namespace PluralityUtilities.Common.Containers
{
	public class QualifiedToken
	{
		public TokenQualifiers Qualifier { get; set; } = TokenQualifiers.Unknown;
		public string Token { get; set; } = string.Empty;

		public QualifiedToken() { }
		public QualifiedToken( QualifiedToken other )
		{
			Qualifier = other.Qualifier;
			Token = other.Token;
		}
		public QualifiedToken( string token )
		{
			Token = token;
		}
	}
}
