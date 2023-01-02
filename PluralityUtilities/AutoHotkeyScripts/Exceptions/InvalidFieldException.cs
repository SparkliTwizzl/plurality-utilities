namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class InvalidFieldException : Exception
	{
		public InvalidFieldException() : base() { }
		public InvalidFieldException(string message) : base(message) { }
		public InvalidFieldException(string message, Exception inner) : base(message, inner) { }
	}
}
