namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class MissingFieldException : Exception
	{
		public MissingFieldException() : base() { }
		public MissingFieldException(string message) : base(message) { }
		public MissingFieldException(string message, Exception inner) : base(message, inner) { }
	}
}
