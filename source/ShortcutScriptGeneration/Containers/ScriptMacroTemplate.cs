using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptMacroTemplate : IEquatable<ScriptMacroTemplate>
	{
		public Dictionary<string, string> FindAndReplace { get; set; } = new();
		public string TemplateFindString { get; set; } = string.Empty;
		public string TemplateReplaceString { get; set; } = string.Empty;
		public string TextCase { get; set; } = TemplateTextCases.Default;


		public ScriptMacroTemplate() { }
		public ScriptMacroTemplate( string templateFindString, string templateReplaceString, string textCase, Dictionary<string, string> findAndReplace )
		{
			FindAndReplace = findAndReplace;
			TemplateFindString = templateFindString;
			TemplateReplaceString = templateReplaceString;
			TextCase = textCase;
		}
		public ScriptMacroTemplate( ScriptMacroTemplate other )
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
			var other = obj as ScriptMacroTemplate;
			return Equals( other );
		}

		public bool Equals( ScriptMacroTemplate? other )
		{
			if ( other is null )
			{
				return false;
			}
			return AreFindAndReplacesEqual( other.FindAndReplace ) && TemplateFindString.Equals( other.TemplateFindString ) && TemplateReplaceString.Equals( other.TemplateReplaceString ) && TextCase.Equals( other.TextCase );
		}

		public override int GetHashCode() => FindAndReplace.GetHashCode() ^ TemplateFindString.GetHashCode() ^ TemplateReplaceString.GetHashCode() ^ TextCase.GetHashCode();

		public override string ToString() => $"{TemplateFindString}{ControlSequences.TemplateFindReplaceDivider}{TemplateReplaceString}";

		public static bool operator ==( ScriptMacroTemplate a, ScriptMacroTemplate b ) => a.Equals( b );

		public static bool operator !=( ScriptMacroTemplate a, ScriptMacroTemplate b ) => !a.Equals( b );


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
