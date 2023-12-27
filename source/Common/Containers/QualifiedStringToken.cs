using Petrichor.Common.Enums;


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

		public QualifiedStringToken( string token ) => Value = token;
	}
}
