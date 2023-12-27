namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	public class BracketMismatchException : Exception
	{
		public BracketMismatchException() : base() { }
		public BracketMismatchException( string message ) : base( message ) { }
		public BracketMismatchException( string message, Exception inner ) : base( message, inner ) { }
	}
}
