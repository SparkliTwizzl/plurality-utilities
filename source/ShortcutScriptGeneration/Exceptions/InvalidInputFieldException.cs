namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	public class InvalidInputFieldException : Exception
	{
		public InvalidInputFieldException() : base() { }
		public InvalidInputFieldException( string message ) : base( message ) { }
		public InvalidInputFieldException( string message, Exception inner ) : base( message, inner ) { }
	}
}
