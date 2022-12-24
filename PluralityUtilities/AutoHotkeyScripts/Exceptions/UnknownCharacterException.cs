namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class UnknownCharacterException : Exception
	{
		public UnknownCharacterException() : base() { }
		public UnknownCharacterException(string message) : base(message) { }
		public UnknownCharacterException(string message, Exception inner) : base(message, inner) { }
	}
}
