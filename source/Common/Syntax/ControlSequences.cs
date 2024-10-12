namespace Petrichor.Common.Syntax
{
	/// <summary>
	/// Contains control sequences used for tokenization and parsing.
	/// </summary>
	public static class ControlSequences
	{
		/// <summary>
		/// The escape character used for escaping sequences.
		/// </summary>
		public const char Escape = '\\';
		
		/// <summary>
		/// The sequence that indicates the closing of a token body.
		/// </summary>
		public const char TokenBodyClose = '}';
		
		/// <summary>
		/// The sequence that indicates the opening of a token body.
		/// </summary>
		public const char TokenBodyOpen = '{';
		
		/// <summary>
		/// The sequence that indicates the closing of a find tag.
		/// </summary>
		public const char FindTagClose = ']';
		
		/// <summary>
		/// The sequence that indicates the opening of a find tag.
		/// </summary>
		public const char FindTagOpen = '[';
		
		/// <summary>
		/// The sequence that starts a line comment.
		/// </summary>
		public const string LineComment = "//";
		
		/// <summary>
		/// The sequence that delimits token keys.
		/// </summary>
		public const char TokenKeyDelimiter = ':';
	}
}
