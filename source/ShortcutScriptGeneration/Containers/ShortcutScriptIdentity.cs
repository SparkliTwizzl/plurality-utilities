namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ShortcutScriptIdentity : IEquatable<ShortcutScriptIdentity>
	{
		public string Name { get; set; } = string.Empty;
		public string Tag { get; set; } = string.Empty;


		public ShortcutScriptIdentity() { }
		public ShortcutScriptIdentity( string name, string tag )
		{
			Name = name;
			Tag = tag;
		}


		public static bool operator ==( ShortcutScriptIdentity a, ShortcutScriptIdentity b )
		{
			return a.Equals(b);
		}

		public static bool operator !=( ShortcutScriptIdentity a, ShortcutScriptIdentity b )
		{
			return !a.Equals(b);
		}

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals((ShortcutScriptIdentity)obj);
		}

		public bool Equals(ShortcutScriptIdentity? other)
		{
			if (other is null)
			{
				return false;
			}
			return Name.Equals( other.Name ) && Tag.Equals( other.Tag );
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Tag.GetHashCode();
		}
	}
}
