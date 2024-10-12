namespace Petrichor.ShortcutScriptGeneration.Containers
{
	/// <summary>
	/// Represents the input data for shortcut script generation.
	/// </summary>
	public sealed class ShortcutScriptInput : IEquatable<ShortcutScriptInput>
	{
		/// <summary>
		/// Gets or sets the list of entries to use for shortcut script generation.
		/// </summary>
		public Entry[] Entries { get; set; } = Array.Empty<Entry>();

		/// <summary>
		/// Gets or sets the module option data for shortcut script generation.
		/// </summary>
		public ModuleOptions ModuleOptions { get; set; } = new();

		/// <summary>
		/// Gets or sets the list of plaintext shortcuts.
		/// </summary>
		public string[] Shortcuts { get; set; } = Array.Empty<string>();

		/// <summary>
		/// Gets or sets the list of shortcut templates to apply to entries.
		/// </summary>
		public TextShortcut[] ShortcutTemplates { get; set; } = Array.Empty<TextShortcut>();


		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutScriptInput"/> class.
		/// </summary>
		public ShortcutScriptInput() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutScriptInput"/> class with the specified parameters.
		/// </summary>
		/// <param name="moduleOptions">The module option data.</param>
		/// <param name="entries">The list of entries to generate shortcuts for.</param>
		/// <param name="templates">Optional list of shortcut templates to apply to entries.</param>
		/// <param name="shortcuts">Optional list of plaintext shortcuts.</param>
		public ShortcutScriptInput( ModuleOptions moduleOptions, Entry[] entries, string[]? shortcuts = null, TextShortcut[]? templates = null )
		{
			Entries = entries;
			ModuleOptions = moduleOptions;
			Shortcuts = shortcuts ?? Array.Empty<string>();
			ShortcutTemplates = templates ?? Array.Empty<TextShortcut>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutScriptInput"/> class by copying another instance.
		/// </summary>
		/// <param name="other">The instance to copy.</param>
		public ShortcutScriptInput( ShortcutScriptInput other)
		{
			Entries = other.Entries;
			ModuleOptions = other.ModuleOptions;
			Shortcuts = other.Shortcuts;
			ShortcutTemplates = other.ShortcutTemplates;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ShortcutScriptInput ) obj );
		}

		/// <summary>
		/// Determines whether the specified <see cref="ShortcutScriptInput"/> is equal to the current <see cref="ShortcutScriptInput"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals( ShortcutScriptInput? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Entries.SequenceEqual( other.Entries ) && Shortcuts.SequenceEqual( other.Shortcuts ) && ModuleOptions.Equals( other.ModuleOptions ) && ShortcutTemplates.SequenceEqual( other.ShortcutTemplates );
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Entries.GetHashCode() ^ ModuleOptions.GetHashCode() ^ Shortcuts.GetHashCode() ^ ShortcutTemplates.GetHashCode();

		/// <summary>
		/// Determines whether two specified instances of <see cref="ShortcutScriptInput"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(ShortcutScriptInput a, ShortcutScriptInput b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="ShortcutScriptInput"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(ShortcutScriptInput a, ShortcutScriptInput b) => !a.Equals(b);
	}
}
