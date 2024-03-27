namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptInput : IEquatable<ScriptInput>
	{
		public ScriptEntry[] Entries { get; set; } = Array.Empty<ScriptEntry>();
		public string[] Macros { get; set; } = Array.Empty<string>();
		public ScriptModuleOptions ModuleOptions { get; set; } = new();
		public ScriptMacroTemplate[] Templates { get; set; } = Array.Empty<ScriptMacroTemplate>();


		public ScriptInput() { }
		public ScriptInput( ScriptModuleOptions moduleOptions, ScriptEntry[] entries, ScriptMacroTemplate[] templates, string[]? macros = null )
		{
			Entries = entries;
			Macros = macros ?? Array.Empty<string>();
			ModuleOptions = moduleOptions;
			Templates = templates;
		}
		public ScriptInput( ScriptInput other)
		{
			Entries = other.Entries;
			Macros = other.Macros;
			ModuleOptions = other.ModuleOptions;
			Templates = other.Templates;
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
			return Entries.SequenceEqual( other.Entries ) && Macros.SequenceEqual( other.Macros ) && ModuleOptions.Equals( other.ModuleOptions ) && Templates.SequenceEqual( other.Templates );
		}

		public override int GetHashCode() => Entries.GetHashCode() ^ Macros.GetHashCode() ^ ModuleOptions.GetHashCode() ^ Templates.GetHashCode();
	}
}
