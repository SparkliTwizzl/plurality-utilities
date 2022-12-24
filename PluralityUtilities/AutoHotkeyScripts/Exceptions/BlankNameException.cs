namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class BlankNameException : Exception
	{
		public BlankNameException() : base() { }
		public BlankNameException(string message) : base(message) { }
		public BlankNameException(string message, Exception inner) : base(message, inner) { }
	}
}
