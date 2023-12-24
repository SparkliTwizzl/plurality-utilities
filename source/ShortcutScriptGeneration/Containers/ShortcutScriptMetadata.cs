namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptMetadata
	{
		public string DefaultIconFilePath { get; set; } = string.Empty;
		public string SuspendIconFilePath { get; set; } = string.Empty;


		public ShortcutScriptMetadata() { }
		public ShortcutScriptMetadata( string defaultIconPath, string suspendIconPath )
		{
			DefaultIconFilePath = defaultIconPath;
			SuspendIconFilePath = suspendIconPath;
		}


		public static bool operator ==( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return left.DefaultIconFilePath.Equals( right.DefaultIconFilePath ) && left.SuspendIconFilePath.Equals( right.SuspendIconFilePath );
		}

		public static bool operator !=( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return !left.DefaultIconFilePath.Equals( right.DefaultIconFilePath ) || !left.SuspendIconFilePath.Equals( right.SuspendIconFilePath );
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
			return DefaultIconFilePath.GetHashCode() ^ SuspendIconFilePath.GetHashCode();
		}
	}
}
