namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	public class UnknownTokenException : Exception
	{
		public UnknownTokenException() : base() { }
		public UnknownTokenException( string message ) : base( message ) { }
		public UnknownTokenException( string message, Exception inner ) : base( message, inner ) { }
	}
}
