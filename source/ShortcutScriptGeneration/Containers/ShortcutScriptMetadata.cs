namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptMetadata
	{
		public string DefaultIconPath { get; set; } = string.Empty;
		public string SuspendIconPath { get; set; } = string.Empty;


		public ShortcutScriptMetadata() { }
		public ShortcutScriptMetadata( string defaultIconPath, string suspendIconPath )
		{
			DefaultIconPath = defaultIconPath;
			SuspendIconPath = suspendIconPath;
		}


		public static bool operator ==( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return left.DefaultIconPath.Equals( right.DefaultIconPath ) && left.SuspendIconPath.Equals( right.SuspendIconPath );
		}

		public static bool operator !=( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return !left.DefaultIconPath.Equals( right.DefaultIconPath ) || !left.SuspendIconPath.Equals( right.SuspendIconPath );
		}

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( ShortcutScriptMetadata )obj;
		}

		public override int GetHashCode()
		{
			return DefaultIconPath.GetHashCode() ^ SuspendIconPath.GetHashCode();
		}
	}
}
