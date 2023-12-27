namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptInput
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


		public static bool operator ==(ShortcutScriptInput left, ShortcutScriptInput right)
		{
			return left.Entries.SequenceEqual(right.Entries) && left.Macros.SequenceEqual(right.Macros) && left.Metadata.Equals(right.Metadata) && left.Templates.SequenceEqual(right.Templates);
		}

		public static bool operator !=(ShortcutScriptInput left, ShortcutScriptInput right)
		{
			return !left.Entries.SequenceEqual(right.Entries) || !left.Macros.SequenceEqual(right.Macros) || !left.Metadata.Equals(right.Metadata) || !left.Templates.SequenceEqual(right.Templates);
		}

		public override bool Equals(object? obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return this == (ShortcutScriptInput)obj;
		}

		public override int GetHashCode()
		{
			return Entries.GetHashCode() ^ Macros.GetHashCode() ^ Metadata.GetHashCode() ^ Templates.GetHashCode();
		}
	}
}