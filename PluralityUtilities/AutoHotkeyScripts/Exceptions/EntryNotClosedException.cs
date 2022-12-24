namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class EntryNotClosedException : Exception
	{
		public EntryNotClosedException() : base() { }
		public EntryNotClosedException(string message) : base(message) { }
		public EntryNotClosedException(string message, Exception inner) : base(message, inner) { }
	}
}
