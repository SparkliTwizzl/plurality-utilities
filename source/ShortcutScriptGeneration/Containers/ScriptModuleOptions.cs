namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptModuleOptions : IEquatable<ScriptModuleOptions>
	{
		public string DefaultIconFilePath { get; set; } = string.Empty;
		public string ReloadShortcut { get; set; } = string.Empty;
		public string SuspendIconFilePath { get; set; } = string.Empty;
		public string SuspendShortcut { get; set; } = string.Empty;


		public ScriptModuleOptions() { }
		public ScriptModuleOptions( string defaultIconPath, string suspendIconPath, string reloadShortcut, string suspendShortcut )
		{
			DefaultIconFilePath = defaultIconPath;
			ReloadShortcut = reloadShortcut;
			SuspendIconFilePath = suspendIconPath;
			SuspendShortcut = suspendShortcut;
		}


		public static bool operator ==( ScriptModuleOptions a, ScriptModuleOptions b ) => a.Equals( b );

		public static bool operator !=( ScriptModuleOptions a, ScriptModuleOptions b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ScriptModuleOptions ) obj );
		}

		public bool Equals( ScriptModuleOptions? other )
		{
			if ( other is null )
			{
				return false;
			}
			return DefaultIconFilePath.Equals( other.DefaultIconFilePath ) && ReloadShortcut.Equals( other.ReloadShortcut ) && SuspendIconFilePath.Equals( other.SuspendIconFilePath ) && SuspendShortcut.Equals( other.SuspendShortcut );
		}

		public override int GetHashCode() => DefaultIconFilePath.GetHashCode() ^ ReloadShortcut.GetHashCode() ^ SuspendIconFilePath.GetHashCode() ^ SuspendShortcut.GetHashCode();
	}
}
