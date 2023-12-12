namespace Petrichor.AutoHotkeyScripts.Exceptions
{
	[ Serializable ]
	public class EscapeCharacterMismatchException : Exception
	{
		public EscapeCharacterMismatchException() : base() { }
		public EscapeCharacterMismatchException( string message ) : base( message ) { }
		public EscapeCharacterMismatchException( string message, Exception inner ) : base( message, inner ) { }
	}
}
