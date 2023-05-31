namespace PluralityUtilities.AutoHotkeyScripts.Containers
{
	public class Input
	{
		public Entry[] Entries { get; set; } = { };
		public string[] Templates { get; set; } = { };


		public static bool operator ==( Input left, Input right )
		{
			return left.Entries.SequenceEqual( right.Entries ) && left.Templates.SequenceEqual( right.Templates );
		}

		public static bool operator !=( Input left, Input right )
		{
			return !left.Entries.SequenceEqual( right.Entries ) || !left.Templates.SequenceEqual( right.Templates );
		}

		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( Input )obj;
		}

		public override int GetHashCode()
		{
			return Entries.GetHashCode() ^ Templates.GetHashCode();
		}
	}
}
