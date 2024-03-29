namespace Petrichor.Common.Exceptions
{
	public class TokenCountException : Exception
	{
		public TokenCountException() : base() { }
		public TokenCountException( string message ) : base( message ) { }
		public TokenCountException( string message, Exception inner ) : base( message, inner ) { }
	}
}
