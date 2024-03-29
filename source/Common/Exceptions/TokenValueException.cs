namespace Petrichor.Common.Exceptions
{
	public class TokenValueException : Exception
	{
		public TokenValueException() : base() { }
		public TokenValueException( string message ) : base( message ) { }
		public TokenValueException( string message, Exception inner ) : base( message, inner ) { }
	}
}
