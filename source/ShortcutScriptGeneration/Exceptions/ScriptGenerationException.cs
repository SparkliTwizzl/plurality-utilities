namespace Petrichor.ShortcutScriptGeneration.Exceptions
{
	[ Serializable ]
	public class ScriptGenerationException : Exception
	{
		public ScriptGenerationException() : base() { }
		public ScriptGenerationException( string message ) : base( message ) { }
		public ScriptGenerationException( string message, Exception inner ) : base( message, inner ) { }
	}
}
