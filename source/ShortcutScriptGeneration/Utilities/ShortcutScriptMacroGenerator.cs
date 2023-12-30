using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.LookUpTables;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMacroGenerator : IMacroGenerator
	{
		public ShortcutScriptMacroGenerator() { }


		public string[] Generate( ShortcutScriptInput input )
		{
			var macros = new List<string>();
			foreach ( var entry in input.Entries )
			{
				macros.AddRange( GenerateMacrosFromEntries( input.Templates, entry ) );
			}
			return macros.ToArray();
		}


		private static List<string> GenerateMacrosFromEntries( string[] templates, ShortcutScriptEntry entry )
		{
			var macros = new List<string>();
			foreach ( var identity in entry.Identities )
			{
				macros.AddRange( GenerateMacrosFromIdentities( templates, identity, entry.Pronoun, entry.Decoration ) );
			}
			return macros;
		}

		private static List<string> GenerateMacrosFromIdentities( string[] templates, ShortcutScriptIdentity identity, string pronoun, string decoration )
		{
			var macros = new List<string>();
			foreach ( var template in templates )
			{
				macros.Add( GenerateMacroFromIdentity( template, identity, pronoun, decoration ) );
			}
			return macros;
		}

		private static string GenerateMacroFromIdentity( string template, ShortcutScriptIdentity identity, string pronoun, string decoration )
		{
			var macro = template;
			var fields = new Dictionary<string, string>()
			{
				{ "name", identity.Name },
				{ "tag", identity.Tag },
				{ "pronoun", pronoun },
				{ "decoration", decoration },
			 };
			foreach ( var marker in ShortcutScriptTemplateMarkers.LookUpTable.Select( marker => marker.Value ) )
			{
				macro = macro.Replace( $"`{marker}`", fields[ marker ] );
			}
			return macro;
		}
	}
}
