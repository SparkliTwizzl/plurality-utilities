namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ShortcutScriptMetadata : IEquatable<ShortcutScriptMetadata>
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


		public static bool operator ==( ShortcutScriptMetadata a, ShortcutScriptMetadata b )
		{
			return a.Equals(b);
		}

		public static bool operator !=( ShortcutScriptMetadata a, ShortcutScriptMetadata b )
		{
			return !a.Equals(b);
		}

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals(( ShortcutScriptMetadata )obj);
		}

		public bool Equals( ShortcutScriptMetadata? other )
		{
			if ( other is null )
			{
				return false;
			}
			return DefaultIconFilePath.Equals( other.DefaultIconFilePath ) && ReloadShortcut.Equals( other.ReloadShortcut ) && SuspendIconFilePath.Equals( other.SuspendIconFilePath ) && SuspendShortcut.Equals( other.SuspendShortcut );
		}

		public override int GetHashCode()
		{
			return DefaultIconFilePath.GetHashCode() ^ ReloadShortcut.GetHashCode() ^ SuspendIconFilePath.GetHashCode() ^ SuspendShortcut.GetHashCode();
		}
	}
}
