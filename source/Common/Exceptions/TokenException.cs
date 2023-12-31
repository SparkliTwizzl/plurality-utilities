namespace Petrichor.Common.Exceptions
{
	public class TokenException : Exception
	{
		public TokenException() : base() { }
		public TokenException( string message ) : base( message ) { }
		public TokenException( string message, Exception inner ) : base( message, inner ) { }
	}
}
