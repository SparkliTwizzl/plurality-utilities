using Petrichor.Common.Containers;


namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	/// <summary>
	/// Represents a collection of data token prototypes used in text shortcut script generation.
	/// </summary>
	public static class TokenPrototypes
	{
		/// <summary>
		/// Gets the token representing the <see cref="Containers.Entry.Color"/> entry field.
		/// </summary>
		public static DataToken Color => new()
		{
			Key = "color",
		};

		/// <summary>
		/// Gets the token representing the <see cref="Containers.Entry.Decoration"/> entry field.
		/// </summary>
		public static DataToken Decoration => new()
		{
			Key = "decoration",
		};

		/// <summary>
		/// Gets the token representing the file path for the default icon of the output script.
		/// </summary>
		public static DataToken DefaultIcon => new()
		{
			Key = "default-icon",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing an entry for a shortcut script.
		/// </summary>
		public static DataToken Entry => new()
		{
			Key = "entry",
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing the list of entries for a shortcut script.
		/// </summary>
		public static DataToken EntryList => new()
		{
			Key = "entry-list",
			MaxAllowed = 1,
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing the find keys for a shortcut template.
		/// /// </summary>
		public static DataToken Find => new()
		{
			Key = "find",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Containers.Entry.ID"/> entry field.
		/// </summary>
		public static DataToken ID => new()
		{
			Key = "id",
			MaxAllowed = 1,
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing the value of the <see cref="Containers.Entry.LastName"/> entry field.
		/// </summary>
		public static DataToken LastName => new()
		{
			Key = "last-name",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the tag of the <see cref="Containers.Entry.LastName"/> entry field.
		/// </summary>
		public static DataToken LastTag => new()
		{
			Key = "last-tag",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the value of the <see cref="Containers.Entry.Name"/> entry field.
		/// </summary>
		public static DataToken Name => new()
		{
			Key = "name",
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing the module option values.
		/// </summary>
		public static DataToken ModuleOptions => new()
		{
			Key = "module-options",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Containers.Entry.Pronoun"/> entry field.
		/// </summary>
		public static DataToken Pronoun => new()
		{
			Key = "pronoun",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the keyboard shortcut to reload the output script.
		/// </summary>
		public static DataToken ReloadShortcut => new()
		{
			Key = "reload-shortcut",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the replace values for a shortcut template.
		/// </summary>
		public static DataToken Replace => new()
		{
			Key = "replace",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the file path of the suspend icon of the output script.
		/// </summary>
		public static DataToken SuspendIcon => new()
		{
			Key = "suspend-icon",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the keyboard shortcut to suspend and resume the output script.
		/// </summary>
		public static DataToken SuspendShortcut => new()
		{
			Key = "suspend-shortcut",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the tag of the <see cref="Containers.Entry.Name"/> entry field.
		/// </summary>
		public static DataToken Tag => new()
		{
			Key = "tag",
		};

		/// <summary>
		/// Gets the token representing a plaintext shortcut.
		/// </summary>
		public static DataToken Shortcut => new()
		{
			Key = "shortcut",
		};

		/// <summary>
		/// Gets the token representing the list of text shortcuts.
		/// </summary>
		public static DataToken ShortcutList => new()
		{
			Key = "shortcut-list",
			MaxAllowed = 1,
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing a shortcut template.
		/// </summary>
		public static DataToken ShortcutTemplate => new()
		{
			Key = "shortcut-template",
		};

		/// <summary>
		/// Gets the token representing the text case of a shortcut template.
		/// </summary>
		public static DataToken TextCase => new()
		{
			Key = "text-case",
			MaxAllowed = 1,
		};
	}
}
