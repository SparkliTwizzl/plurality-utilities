namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptIdentity : IEquatable<ScriptIdentity>
	{
		public static readonly ScriptIdentity Empty = new ScriptIdentity();
		public string Name { get; set; } = string.Empty;
		public string Tag { get; set; } = string.Empty;


		public ScriptIdentity() { }
		public ScriptIdentity( string name, string tag )
		{
			Name = name;
			Tag = tag;
		}


		public static bool operator ==( ScriptIdentity a, ScriptIdentity b ) => a.Equals( b );

		public static bool operator !=( ScriptIdentity a, ScriptIdentity b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ScriptIdentity ) obj );
		}

		public bool Equals( ScriptIdentity? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Name.Equals( other.Name ) && Tag.Equals( other.Tag );
		}

		public override int GetHashCode() => Name.GetHashCode() ^ Tag.GetHashCode();
	}
}
