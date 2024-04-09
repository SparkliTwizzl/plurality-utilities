namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class Identity : IEquatable<Identity>
	{
		public static readonly Identity Empty = new Identity();
		public string Name { get; set; } = string.Empty;
		public string Tag { get; set; } = string.Empty;


		public Identity() { }
		public Identity( string name, string tag )
		{
			Name = name;
			Tag = tag;
		}


		public static bool operator ==( Identity a, Identity b ) => a.Equals( b );

		public static bool operator !=( Identity a, Identity b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( Identity ) obj );
		}

		public bool Equals( Identity? other )
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
