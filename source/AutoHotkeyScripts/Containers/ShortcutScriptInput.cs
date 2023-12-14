namespace Petrichor.AutoHotkeyScripts.Containers
{
	public class ShortcutScriptInput
	{
		public ShortcutScriptEntry[] Entries { get; set; } = { };
		public string[] Templates { get; set; } = { };


		public ShortcutScriptInput() { }
		public ShortcutScriptInput( ShortcutScriptEntry[] entries, string[] templates )
		{
			Entries = entries;
			Templates = templates;
		}


		public static bool operator ==( ShortcutScriptInput left, ShortcutScriptInput right )
		{
			return left.Entries.SequenceEqual( right.Entries ) && left.Templates.SequenceEqual( right.Templates );
		}

		public static bool operator !=( ShortcutScriptInput left, ShortcutScriptInput right )
		{
			return !left.Entries.SequenceEqual( right.Entries ) || !left.Templates.SequenceEqual( right.Templates );
		}

		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( ShortcutScriptInput )obj;
		}

		public override int GetHashCode()
		{
			return Entries.GetHashCode() ^ Templates.GetHashCode();
		}
	}
}
