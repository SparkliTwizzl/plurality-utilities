using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.LookUpTables;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMacroParser : IShortcutScriptMacroParser
	{
		public ShortcutScriptMacroParser() { }


		public string[] GenerateMacrosFromInput(ShortcutScriptInput input)
		{
			var macros = new List<string>();
			foreach (var entry in input.Entries)
			{
				macros.AddRange(GenerateAllEntryMacrosFromTemplates(input.Templates, entry));
			}
			return macros.ToArray();
		}


		private List<string> GenerateAllIdentityMacrosFromTemplates(string[] templates, ShortcutScriptIdentity identity, string pronoun, string decoration)
		{
			var macros = new List<string>();
			foreach (var template in templates)
			{
				macros.Add(GenerateIdentityMacroFromTemplate(template, identity, pronoun, decoration));
			}
			return macros;
		}

		private List<string> GenerateAllEntryMacrosFromTemplates(string[] templates, ShortcutScriptEntry entry)
		{
			var macros = new List<string>();
			foreach (var identity in entry.Identities)
			{
				macros.AddRange(GenerateAllIdentityMacrosFromTemplates(templates, identity, entry.Pronoun, entry.Decoration));
			}
			return macros;
		}

		private string GenerateIdentityMacroFromTemplate(string template, ShortcutScriptIdentity identity, string pronoun, string decoration)
		{
			var macro = template;
			Dictionary<string, string> fields = new Dictionary<string, string>()
			{
				{ "name", identity.Name },
				{ "tag", identity.Tag },
				{ "pronoun", pronoun },
				{ "decoration", decoration },
			 };
			foreach (var marker in ShortcutScriptTemplateMarkers.LookUpTable.Select(marker => marker.Value))
			{
				macro = macro.Replace($"`{marker}`", fields[marker]);
			}
			return macro;
		}
	}
}
