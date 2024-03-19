namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	public class EscapeCharacterException : Exception
	{
		public EscapeCharacterException() : base() { }
		public EscapeCharacterException( string message ) : base( message ) { }
		public EscapeCharacterException( string message, Exception inner ) : base( message, inner ) { }
	}
}
