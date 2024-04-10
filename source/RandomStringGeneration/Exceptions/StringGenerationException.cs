namespace Petrichor.RandomStringGeneration.Exceptions
{
	public class StringGenerationException : Exception
	{
		public StringGenerationException() : base() { }
		public StringGenerationException( string message ) : base( message ) { }
		public StringGenerationException( string message, Exception inner ) : base( message, inner ) { }
	}
}
