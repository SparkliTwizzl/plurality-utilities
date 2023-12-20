namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	[ Serializable ]
	public class DuplicateInputFieldException : Exception
	{
		public DuplicateInputFieldException() : base() { }
		public DuplicateInputFieldException( string message ) : base( message ) { }
		public DuplicateInputFieldException( string message, Exception inner ) : base( message, inner ) { }
	}
}
