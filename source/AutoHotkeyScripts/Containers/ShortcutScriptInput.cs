namespace Petrichor.AutoHotkeyScripts.Containers
{
	public class ShortcutScriptInput
	{
		public ShortcutScriptEntry[] Entries { get; set; } = { };
		public ShortcutScriptMetadata Metadata { get; set; } = new ShortcutScriptMetadata();
		public string[] Templates { get; set; } = { };


		public ShortcutScriptInput() { }
		public ShortcutScriptInput( ShortcutScriptEntry[] entries, ShortcutScriptMetadata metadata, string[] templates )
		{
			Entries = entries;
			Metadata = metadata;
			Templates = templates;
		}


		public static bool operator ==( ShortcutScriptInput left, ShortcutScriptInput right )
		{
			return left.Entries.SequenceEqual( right.Entries ) && left.Metadata.Equals( right.Metadata ) && left.Templates.SequenceEqual( right.Templates );
		}

		public static bool operator !=( ShortcutScriptInput left, ShortcutScriptInput right )
		{
			return !left.Entries.SequenceEqual( right.Entries ) || !left.Metadata.Equals( right.Metadata ) || !left.Templates.SequenceEqual( right.Templates );
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
			return Entries.GetHashCode() ^ Metadata.GetHashCode() ^ Templates.GetHashCode();
		}
	}
}
