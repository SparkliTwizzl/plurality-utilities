using Petrichor.Common.Containers;


namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	public readonly struct Tokens
	{
		public static DataToken Color => new()
		{
			Key = "color",
		};
		public static DataToken Decoration => new()
		{
			Key = "decoration",
		};
		public static DataToken DefaultIcon => new()
		{
			Key = "default-icon",
			MaxAllowed = 1,
		};
		public static DataToken Entry => new()
		{
			Key = "entry",
			MinRequired = 1,
		};
		public static DataToken EntryList => new()
		{
			Key = "entry-list",
			MaxAllowed = 1,
			MinRequired = 1,
		};
		public static DataToken Find => new()
		{
			Key = "find",
			MaxAllowed = 1,
		};
		public static DataToken ID => new()
		{
			Key = "id",
			MaxAllowed = 1,
			MinRequired = 1,
		};
		public static DataToken LastName => new()
		{
			Key = "last-name",
			MaxAllowed = 1,
		};
		public static DataToken LastTag => new()
		{
			Key = "last-tag",
			MaxAllowed = 1,
		};
		public static DataToken Name => new()
		{
			Key = "name",
			MinRequired = 1,
		};
		public static DataToken ModuleOptions => new()
		{
			Key = "module-options",
			MaxAllowed = 1,
		};
		public static DataToken Pronoun => new()
		{
			Key = "pronoun",
			MaxAllowed = 1,
		};
		public static DataToken ReloadShortcut => new()
		{
			Key = "reload-shortcut",
			MaxAllowed = 1,
		};
		public static DataToken Replace => new()
		{
			Key = "replace",
			MaxAllowed = 1,
		};
		public static DataToken SuspendIcon => new()
		{
			Key = "suspend-icon",
			MaxAllowed = 1,
		};
		public static DataToken SuspendShortcut => new()
		{
			Key = "suspend-shortcut",
			MaxAllowed = 1,
		};
		public static DataToken Tag => new()
		{
			Key = "tag",
		};
		public static DataToken Template => new()
		{
			Key = "template",
			MinRequired = 1,
		};
		public static DataToken TemplateList => new()
		{
			Key = "template-list",
			MaxAllowed = 1,
			MinRequired = 1,
		};


		public static Dictionary<string, DataToken> LookUpTable => new()
		{
			{ Color.Key, Color },
			{ Decoration.Key, Decoration },
			{ DefaultIcon.Key, DefaultIcon },
			{ Entry.Key, Entry },
			{ EntryList.Key, EntryList },
			{ ID.Key, ID },
			{ LastName.Key, LastName },
			{ LastTag.Key, LastTag },
			{ Name.Key, Name },
			{ ModuleOptions.Key, ModuleOptions },
			{ Pronoun.Key, Pronoun },
			{ ReloadShortcut.Key, ReloadShortcut },
			{ SuspendIcon.Key, SuspendIcon },
			{ SuspendShortcut.Key, SuspendShortcut },
			{ Tag.Key, Tag },
			{ Template.Key, Template },
			{ TemplateList.Key, TemplateList },
		};
	}
}
