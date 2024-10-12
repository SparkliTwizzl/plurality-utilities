using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Containers
{
	/// <summary>
	/// Represents a plaintext or templated text shortcut.
	/// </summary>
	public sealed class TextShortcut : IEquatable<TextShortcut>
	{
		/// <summary>
		/// Gets or sets a dictionary of 
		/// </summary>
		public Dictionary<string, string> FindAndReplace { get; set; } = new();

		/// <summary>
		/// Gets or sets the plaintext shortcut string.
		/// </summary>
		public string ShortcutString { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the find string of a templated shortcut.
		/// </summary>
		public string TemplateFindString { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the replace string of a templated shortcut.
		/// </summary>
		public string TemplateReplaceString { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the text case of a templated shortcut.
		/// </summary>
		public string TextCase { get; set; } = TemplateTextCases.Default;


		/// <summary>
		/// Initializes a new instance of the <see cref="TextShortcut"/> class.
		/// </summary>
		public TextShortcut() { }

		/// <summary>
		/// Initializes a new instace of the <see cref="TextShortcut"/> class with the specified shortcut string.
		/// </summary>
		/// <param name="shortcutString">The shortcut string to store.</param>
		public TextShortcut( string shortcutString ) => ShortcutString = shortcutString;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextShortcut"/> class with the specified parameters.
		/// </summary>
		/// <param name="templateFindString">The find string for this template.</param>
		/// <param name="templateReplaceString">The replace string for this template.</param>
		/// <param name="textCase">The text case to apply with this template.</param>
		/// <param name="findAndReplace">The find and replace dictionary for this template.</param>
		public TextShortcut( string templateFindString, string templateReplaceString, string textCase, Dictionary<string, string> findAndReplace )
		{
			FindAndReplace = findAndReplace;
			TemplateFindString = templateFindString;
			TemplateReplaceString = templateReplaceString;
			TextCase = textCase;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextShortcut"/> class by copying another instance.
		/// </summary>
		/// <param name="other">The instance to copy.</param>
		public TextShortcut( TextShortcut other )
		{
			FindAndReplace = other.FindAndReplace;
			TemplateReplaceString = other.TemplateReplaceString;
			TextCase = other.TextCase;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}
			var other = obj as TextShortcut;
			return Equals( other );
		}

		/// <summary>
		/// Determines whether the specified <see cref="TextShortcut"/> is equal to the current <see cref="TextShortcut"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals( TextShortcut? other )
		{
			if ( other is null )
			{
				return false;
			}
			return AreFindAndReplacesEqual( other.FindAndReplace ) && ShortcutString.Equals( other.ShortcutString ) && TemplateFindString.Equals( other.TemplateFindString ) && TemplateReplaceString.Equals( other.TemplateReplaceString ) && TextCase.Equals( other.TextCase );
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => FindAndReplace.GetHashCode() ^ ShortcutString.GetHashCode() ^ TemplateFindString.GetHashCode() ^ TemplateReplaceString.GetHashCode() ^ TextCase.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString() => $"shortcut:[{ShortcutString}] template:[{TemplateFindString}{ControlSequences.TemplateFindStringDelimiter}{TemplateReplaceString}]";

		/// <summary>
		/// Determines whether two specified instances of <see cref="TextShortcut"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==( TextShortcut a, TextShortcut b ) => a.Equals( b );

		/// <summary>
		/// Determines whether two specified instances of <see cref="TextShortcut"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=( TextShortcut a, TextShortcut b ) => !a.Equals( b );


		/// <summary>
		/// Checks if this <see cref="TextShortcut"/> instance's find and replace dictionary matches another.
		/// </summary>
		/// <param name="other"></param>
		/// <returns>True if find and replace dictionaries match, otherwise false.</returns>
		private bool AreFindAndReplacesEqual( Dictionary<string, string> other )
		{
			if ( FindAndReplace.Count != other.Count )
			{
				return false;
			}

			if ( FindAndReplace.Keys.Except( other.Keys ).Any() )
			{
				return false;
			}

			if ( other.Keys.Except( FindAndReplace.Keys ).Any() )
			{
				return false;
			}

			foreach ( var item in FindAndReplace )
			{
				if ( !item.Value.Equals( other[ item.Key ] ) )
				{
					return false;
				}
			}

			return true;
		}
	}
}
