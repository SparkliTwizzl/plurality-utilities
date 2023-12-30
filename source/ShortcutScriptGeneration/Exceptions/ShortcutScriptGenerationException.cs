namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	public class ShortcutScriptGenerationException : Exception
	{
		public ShortcutScriptGenerationException() : base() { }
		public ShortcutScriptGenerationException( string message ) : base( message ) { }
		public ShortcutScriptGenerationException( string message, Exception inner ) : base( message, inner ) { }
	}
}
