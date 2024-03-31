namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptInput : IEquatable<ScriptInput>
	{
		public ScriptEntry[] Entries { get; set; } = Array.Empty<ScriptEntry>();
		public ScriptModuleOptions ModuleOptions { get; set; } = new();
		public string[] Shortcuts { get; set; } = Array.Empty<string>();
		public ScriptShortcutData[] ShortcutTemplates { get; set; } = Array.Empty<ScriptShortcutData>();


		public ScriptInput() { }
		public ScriptInput( ScriptModuleOptions moduleOptions, ScriptEntry[] entries, ScriptShortcutData[] templates, string[]? shortcuts = null )
		{
			Entries = entries;
			ModuleOptions = moduleOptions;
			Shortcuts = shortcuts ?? Array.Empty<string>();
			ShortcutTemplates = templates;
		}
		public ScriptInput( ScriptInput other)
		{
			Entries = other.Entries;
			ModuleOptions = other.ModuleOptions;
			Shortcuts = other.Shortcuts;
			ShortcutTemplates = other.ShortcutTemplates;
		}


		public static bool operator ==( ScriptInput a, ScriptInput b ) => a.Equals( b );

		public static bool operator !=( ScriptInput a, ScriptInput b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ScriptInput ) obj );
		}

		public bool Equals( ScriptInput? other )
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
