namespace PluralityUtilities.AutoHotkeyScripts.Exceptions
{
	[Serializable]
	public class UnexpectedCharacterException : Exception
	{
		public UnexpectedCharacterException() : base() { }
		public UnexpectedCharacterException(string message) : base(message) { }
		public UnexpectedCharacterException(string message, Exception inner) : base(message, inner) { }
	}
}
