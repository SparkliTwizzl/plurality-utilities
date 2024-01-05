using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.ShortcutScriptGeneration.LookUpTables;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class MacroGenerator : IMacroGenerator
	{
		public string[] Generate( ScriptInput input )
		{
			var macros = new List<string>();
			foreach ( var entry in input.Entries )
			{
				macros.AddRange( GenerateMacrosFromEntries( input.Templates, entry ) );
			}
			return macros.ToArray();
		}


		private static List<string> GenerateMacrosFromEntries( string[] templates, ScriptEntry entry )
		{
			var macros = new List<string>();
			foreach ( var identity in entry.Identities )
			{
				macros.AddRange( GenerateMacrosFromIdentities( templates, identity, entry.Pronoun, entry.Decoration ) );
			}
			return macros;
		}

		private static List<string> GenerateMacrosFromIdentities( string[] templates, ScriptIdentity identity, string pronoun, string decoration )
		{
			var macros = new List<string>();
			foreach ( var template in templates )
			{
				macros.Add( GenerateMacroFromIdentity( template, identity, pronoun, decoration ) );
			}
			return macros;
		}

		private static string GenerateMacroFromIdentity( string template, ScriptIdentity identity, string pronoun, string decoration )
		{
			var macro = template;
			var fields = new Dictionary<string, string>()
			{
				{ ShortcutScriptGenerationSyntax.TemplateFindDecorationString, decoration },
				{ ShortcutScriptGenerationSyntax.TemplateFindNameString, identity.Name },
				{ ShortcutScriptGenerationSyntax.TemplateFindPronounString, pronoun },
				{ ShortcutScriptGenerationSyntax.TemplateFindTagString, identity.Tag },
			 };
			foreach ( var findString in ScriptTemplateFindStrings.LookUpTable )
			{
				macro = macro.Replace( $"{findString}", fields[ findString ] )
					.Replace( $"\\{ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar}",
						ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar.ToString() )
					.Replace( $"\\{ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar}",
						ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar.ToString() );
			}
			return macro;
		}
	}
}
