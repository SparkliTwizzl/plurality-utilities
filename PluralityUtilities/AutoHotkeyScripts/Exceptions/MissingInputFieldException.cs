namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[ Serializable ]
	public class MissingInputFieldException : Exception
	{
		public MissingInputFieldException() : base() { }
		public MissingInputFieldException( string message ) : base( message ) { }
		public MissingInputFieldException( string message, Exception inner ) : base( message, inner ) { }
	}
}
