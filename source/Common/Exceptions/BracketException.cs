namespace Petrichor.Common.Exceptions
{
	public class BracketException : Exception
	{
		public BracketException() : base() { }
		public BracketException( string message ) : base( message ) { }
		public BracketException( string message, Exception inner ) : base( message, inner ) { }
	}
}
