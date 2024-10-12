namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	/// <summary>
	/// Contains text cases for templated text shortcuts.
	/// </summary>
	public static class TemplateTextCases
	{
		/// <summary>
		/// The default text case if not specified by the user.
		/// </summary>
		public const string Default = Unchanged;

		/// <summary>
		/// Indicates "First Caps" case should be used.
		/// </summary>
		public const string FirstCaps = "firstCaps";

		/// <summary>
		/// Indicates "lower" case should be used.
		/// </summary>
		public const string Lower = "lower";

		/// <summary>
		/// Indicates text case should not be modified.
		/// </summary>
		public const string Unchanged = "unchanged";

		/// <summary>
		/// Indicates "UPPER" case should be used.
		/// </summary>
		public const string Upper = "upper";


		/// <summary>
		/// A lookup table of all defined cases.
		/// </summary>
		public static string[] LookupTable =>
		[
			FirstCaps,
			Lower,
			Unchanged,
			Upper,
		];
	}
}
