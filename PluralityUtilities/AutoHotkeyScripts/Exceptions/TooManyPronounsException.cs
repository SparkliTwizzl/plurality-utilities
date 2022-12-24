namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class TooManyPronounsException : Exception
	{
		public TooManyPronounsException() : base() { }
		public TooManyPronounsException(string message) : base(message) { }
		public TooManyPronounsException(string message, Exception inner) : base(message, inner) { }
	}
}
