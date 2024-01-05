namespace Petrichor.Common.Exceptions
{
	public class FileRegionException : Exception
	{
		public FileRegionException() : base() { }
		public FileRegionException( string message ) : base( message ) { }
		public FileRegionException( string message, Exception inner ) : base( message, inner ) { }
	}
}
