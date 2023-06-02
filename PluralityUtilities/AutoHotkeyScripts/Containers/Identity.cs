namespace PluralityUtilities.AutoHotkeyScripts.Containers
{
	public class Identity
	{
		public string Name { get; set; } = string.Empty;
		public string Tag { get; set; } = string.Empty;


		public Identity() { }
		public Identity( string name, string tag )
		{
			Name = name;
			Tag = tag;
		}


		public static bool operator ==( Identity left, Identity right )
		{
			return left.Name.Equals( right.Name ) && left.Tag.Equals( right.Tag );
		}

		public static bool operator !=( Identity left, Identity right )
		{
			return !left.Name.Equals( right.Name ) || !left.Tag.Equals( right.Tag );
		}

		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( Identity )obj;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Tag.GetHashCode();
		}
	}
}
