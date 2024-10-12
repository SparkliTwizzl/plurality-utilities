using Petrichor.Common.Containers;


namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	/// <summary>
	/// Contains predefined tags for find-and-replace in templated shortcuts.
	/// </summary>
	public static class TemplateFindTags
	{
		/// <summary>
		/// Gets the tag corresponding to the <see cref="Containers.Entry.Color"> field.
		/// </summary>
		public static string Color => QualifyTag(TokenPrototypes.Color);

		/// <summary>
		/// Gets the tag corresponding to the <see cref="Containers.Entry.Decoration"> field.
		/// </summary>
		public static string Decoration => QualifyTag(TokenPrototypes.Decoration);

		/// <summary>
		/// Gets the tag corresponding to the <see cref="Containers.Entry.ID"> field.
		/// </summary>
		public static string ID => QualifyTag(TokenPrototypes.ID);

		/// <summary>
		/// Gets the tag corresponding to the value of the <see cref="Containers.Entry.Name"> field.
		/// </summary>
		public static string Name => QualifyTag(TokenPrototypes.Name);

		/// <summary>
		/// Gets the tag corresponding to the value of the <see cref="Containers.Entry.LastName"> field.
		/// </summary>
		public static string LastName => QualifyTag(TokenPrototypes.LastName);

		/// <summary>
		/// Gets the tag corresponding to the tag of the <see cref="Containers.Entry.LastName"> field.
		/// </summary>
		public static string LastTag => QualifyTag(TokenPrototypes.LastTag);

		/// <summary>
		/// Gets the tag corresponding to the <see cref="Containers.Entry.Pronoun"> field.
		/// </summary>
		public static string Pronoun => QualifyTag(TokenPrototypes.Pronoun);

		/// <summary>
		/// Gets the tag corresponding to the tag of the <see cref="Containers.Entry.Name"> field.
		/// </summary>
		public static string Tag => QualifyTag(TokenPrototypes.Tag);


		/// <summary>
		/// Gets a lookup table of all defined tags.
		/// </summary>
		public static string[] LookupTable =>
		[
			Color,
			Decoration,
			ID,
			Name,
			LastName,
			LastTag,
			Pronoun,
			Tag,
		];


		private static string QualifyTag(DataToken tagToken) => $"{Common.Syntax.ControlSequences.FindTagOpen}{tagToken.Key}{Common.Syntax.ControlSequences.FindTagClose}";
	}
}
