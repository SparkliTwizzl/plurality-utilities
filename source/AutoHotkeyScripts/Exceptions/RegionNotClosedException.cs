namespace Petrichor.AutoHotkeyScripts.Exceptions
{
	[ Serializable ]
	public class RegionNotClosedException : Exception
	{
		public RegionNotClosedException() : base() { }
		public RegionNotClosedException( string message ) : base( message ) { }
		public RegionNotClosedException( string message, Exception inner ) : base( message, inner ) { }
	}
}
