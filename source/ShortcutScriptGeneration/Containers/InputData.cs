namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class InputData : IEquatable<InputData>
	{
		public Entry[] Entries { get; set; } = Array.Empty<Entry>();
		public ModuleOptionData ModuleOptions { get; set; } = new();
		public string[] Shortcuts { get; set; } = Array.Empty<string>();
		public ShortcutData[] ShortcutTemplates { get; set; } = Array.Empty<ShortcutData>();


		public InputData() { }
		public InputData( ModuleOptionData moduleOptions, Entry[] entries, ShortcutData[] templates, string[]? shortcuts = null )
		{
			Entries = entries;
			ModuleOptions = moduleOptions;
			Shortcuts = shortcuts ?? Array.Empty<string>();
			ShortcutTemplates = templates;
		}
		public InputData( InputData other)
		{
			Entries = other.Entries;
			ModuleOptions = other.ModuleOptions;
			Shortcuts = other.Shortcuts;
			ShortcutTemplates = other.ShortcutTemplates;
		}


		public static bool operator ==( InputData a, InputData b ) => a.Equals( b );

		public static bool operator !=( InputData a, InputData b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( InputData ) obj );
		}

		public bool Equals( InputData? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Entries.SequenceEqual( other.Entries ) && Shortcuts.SequenceEqual( other.Shortcuts ) && ModuleOptions.Equals( other.ModuleOptions ) && ShortcutTemplates.SequenceEqual( other.ShortcutTemplates );
		}

		public override int GetHashCode() => Entries.GetHashCode() ^ ModuleOptions.GetHashCode() ^ Shortcuts.GetHashCode() ^ ShortcutTemplates.GetHashCode();
	}
}
