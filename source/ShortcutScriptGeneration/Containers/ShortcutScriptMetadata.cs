namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptMetadata
	{
		public string DefaultIconFilePath { get; set; } = string.Empty;
		public string ReloadShortcut { get; set; } = string.Empty;
		public string SuspendIconFilePath { get; set; } = string.Empty;
		public string SuspendShortcut { get; set; } = string.Empty;


		public ShortcutScriptMetadata() { }
		public ShortcutScriptMetadata( string defaultIconPath, string suspendIconPath, string reloadShortcut, string suspendShortcut )
		{
			DefaultIconFilePath = defaultIconPath;
			ReloadShortcut = reloadShortcut;
			SuspendIconFilePath = suspendIconPath;
			SuspendShortcut = suspendShortcut;
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
			return DefaultIconFilePath.GetHashCode() ^ ReloadShortcut.GetHashCode() ^ SuspendIconFilePath.GetHashCode() ^ SuspendShortcut.GetHashCode();
		}
	}
}
