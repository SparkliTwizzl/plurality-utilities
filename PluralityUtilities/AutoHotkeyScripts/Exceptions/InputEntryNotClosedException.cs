namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class InputEntryNotClosedException : Exception
	{
		public InputEntryNotClosedException() : base() { }
		public InputEntryNotClosedException(string message) : base(message) { }
		public InputEntryNotClosedException(string message, Exception inner) : base(message, inner) { }
	}
}
