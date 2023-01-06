namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class DuplicateFieldException : Exception
	{
		public DuplicateFieldException() : base() { }
		public DuplicateFieldException(string message) : base(message) { }
		public DuplicateFieldException(string message, Exception inner) : base(message, inner) { }
	}
}
