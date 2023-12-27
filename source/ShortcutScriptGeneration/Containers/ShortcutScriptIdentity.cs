namespace Petrichor.ShortcutScriptGeneration.Containers
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


		public override bool Equals( object? obj )
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
