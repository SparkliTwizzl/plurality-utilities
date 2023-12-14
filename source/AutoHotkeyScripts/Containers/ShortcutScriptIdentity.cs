namespace Petrichor.AutoHotkeyScripts.Containers
{
	public class ShortcutScriptIdentity
	{
		public string Name { get; set; } = string.Empty;
		public string Tag { get; set; } = string.Empty;


		public ShortcutScriptIdentity() { }
		public ShortcutScriptIdentity( string name, string tag )
		{
			Name = name;
			Tag = tag;
		}


		public static bool operator ==( ShortcutScriptIdentity left, ShortcutScriptIdentity right )
		{
			return left.Name.Equals( right.Name ) && left.Tag.Equals( right.Tag );
		}

		public static bool operator !=( ShortcutScriptIdentity left, ShortcutScriptIdentity right )
		{
			return !left.Name.Equals( right.Name ) || !left.Tag.Equals( right.Tag );
		}

		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( ShortcutScriptIdentity )obj;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Tag.GetHashCode();
		}
	}
}
