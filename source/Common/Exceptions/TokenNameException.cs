namespace Petrichor.Common.Exceptions
{
	public class TokenNameException : Exception
	{
		public TokenNameException() : base() { }
		public TokenNameException( string message ) : base( message ) { }
		public TokenNameException( string message, Exception inner ) : base( message, inner ) { }
	}
}
