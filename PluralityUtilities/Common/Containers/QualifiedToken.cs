using PluralityUtilities.Common.Enums;

namespace PluralityUtilities.Common.Containers
{
	public class QualifiedToken
	{
		public TokenQualifiers Qualifier { get; set; } = TokenQualifiers.Unknown;
		public string Value { get; set; } = string.Empty;

		public QualifiedToken() { }
		public QualifiedToken( QualifiedToken other )
		{
			Qualifier = other.Qualifier;
			Value = other.Value;
		}
		public QualifiedToken( string token )
		{
			Value = token;
		}
	}
}
