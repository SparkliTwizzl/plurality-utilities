using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptShortcutData : IEquatable<ScriptShortcutData>
	{
		public Dictionary<string, string> FindAndReplace { get; set; } = new();
		public string ShortcutString { get; set; } = string.Empty;
		public string TemplateFindString { get; set; } = string.Empty;
		public string TemplateReplaceString { get; set; } = string.Empty;
		public string TextCase { get; set; } = TemplateTextCases.Default;


		public ScriptShortcutData() { }
		public ScriptShortcutData( string shortcutString ) => ShortcutString = shortcutString;
		public ScriptShortcutData( string templateFindString, string templateReplaceString, string textCase, Dictionary<string, string> findAndReplace )
		{
			FindAndReplace = findAndReplace;
			TemplateFindString = templateFindString;
			TemplateReplaceString = templateReplaceString;
			TextCase = textCase;
		}
		public ScriptShortcutData( ScriptShortcutData other )
		{
			FindAndReplace = other.FindAndReplace;
			TemplateReplaceString = other.TemplateReplaceString;
			TextCase = other.TextCase;
		}


		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}
			var other = obj as ScriptShortcutData;
			return Equals( other );
		}

		public bool Equals( ScriptShortcutData? other )
		{
			if ( other is null )
			{
				return false;
			}
			return AreFindAndReplacesEqual( other.FindAndReplace ) && ShortcutString.Equals( other.ShortcutString ) && TemplateFindString.Equals( other.TemplateFindString ) && TemplateReplaceString.Equals( other.TemplateReplaceString ) && TextCase.Equals( other.TextCase );
		}

		public override int GetHashCode() => FindAndReplace.GetHashCode() ^ ShortcutString.GetHashCode() ^ TemplateFindString.GetHashCode() ^ TemplateReplaceString.GetHashCode() ^ TextCase.GetHashCode();

		public override string ToString() => $"shortcut:[{ShortcutString}] template:[{TemplateFindString}{ControlSequences.ShortcutFindReplaceDivider}{TemplateReplaceString}]";

		public static bool operator ==( ScriptShortcutData a, ScriptShortcutData b ) => a.Equals( b );

		public static bool operator !=( ScriptShortcutData a, ScriptShortcutData b ) => !a.Equals( b );


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
