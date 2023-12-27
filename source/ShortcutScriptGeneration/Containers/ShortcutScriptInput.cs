namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ShortcutScriptInput : IEquatable<ShortcutScriptInput>
	{
		public ShortcutScriptEntry[] Entries { get; set; } = Array.Empty<ShortcutScriptEntry>();
		public string[] Macros { get; set; } = Array.Empty<string>();
		public ShortcutScriptMetadata Metadata { get; set; } = new();
		public string[] Templates { get; set; } = Array.Empty<string>();


		public ShortcutScriptInput() { }
		public ShortcutScriptInput(ShortcutScriptMetadata metadata, ShortcutScriptEntry[] entries, string[] templates)
		{
			Entries = entries;
			Macros = Array.Empty<string>();
			Metadata = metadata;
			Templates = templates;
		}
		public ShortcutScriptInput(ShortcutScriptMetadata metadata, ShortcutScriptEntry[] entries, string[] templates, string[] macros)
		{
			Entries = entries;
			Macros = macros;
			Metadata = metadata;
			Templates = templates;
		}


		public static bool operator ==(ShortcutScriptInput a, ShortcutScriptInput b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ShortcutScriptInput a, ShortcutScriptInput b)
		{
			return !a.Equals(b);
		}

		public override bool Equals(object? obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return Equals( (ShortcutScriptInput)obj );
		}

		public bool Equals(ShortcutScriptInput? other)
		{
			if (other is null)
			{
				return false;
			}
			return Entries.SequenceEqual(other.Entries) && Macros.SequenceEqual(other.Macros) && Metadata.Equals(other.Metadata) && Templates.SequenceEqual(other.Templates);
		}

		public override int GetHashCode()
		{
			return Entries.GetHashCode() ^ Macros.GetHashCode() ^ Metadata.GetHashCode() ^ Templates.GetHashCode();
		}
	}
}