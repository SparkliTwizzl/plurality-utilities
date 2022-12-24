namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class BlankFieldException : Exception
	{
		public BlankFieldException() : base() { }
		public BlankFieldException(string message) : base(message) { }
		public BlankFieldException(string message, Exception inner) : base(message, inner) { }
	}
}
