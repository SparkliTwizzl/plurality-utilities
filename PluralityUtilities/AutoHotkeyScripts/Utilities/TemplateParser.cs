using System.Text;

using PluralityUtilities.AutoHotkeyScripts.Containers;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class TemplateParser
	{
		public static string CreateMacroFromTemplate(string template, Identity identity, string pronoun, string decoration)
		{
			StringBuilder macro = new StringBuilder();
			foreach (char c in template)
			{
				switch (c)
				{
					case '#':
						macro.Append(identity.Name);
						break;
					case '@':
						macro.Append(identity.Tag);
						break;
					case '$':
						macro.Append(pronoun);
						break;
					case '&':
						macro.Append(decoration);
						break;
					default:
						macro.Append(c);
						break;
				}
			}
			return macro.ToString();
		}
	}
}
