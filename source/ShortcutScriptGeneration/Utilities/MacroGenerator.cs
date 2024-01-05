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
			foreach ( var identity in entry.Identities.ToList())
			{
				var batch = new ScriptEntry( entry );
				batch.Identities.Clear();
				batch.Identities.Add( identity );
				macros.AddRange( GenerateMacrosFromIdentity( templates, batch ) );
			}
			return macros;
		}

		private static List<string> GenerateMacrosFromIdentity( string[] templates, ScriptEntry entry )
		{
			var macros = new List<string>();
			foreach ( var template in templates )
			{
				macros.Add( GenerateMacroFromTemplate( template, entry ) );
			}
			return macros;
		}

		private static string GenerateMacroFromTemplate( string template, ScriptEntry entry )
		{
			var macro = template;
			var fields = new Dictionary<string, string>()
			{
				{ ShortcutScriptGenerationSyntax.TemplateFindColorString, entry.Color },
				{ ShortcutScriptGenerationSyntax.TemplateFindDecorationString, entry.Decoration },
				{ ShortcutScriptGenerationSyntax.TemplateFindIDString, entry.ID },
				{ ShortcutScriptGenerationSyntax.TemplateFindNameString, entry.Identities[ 0 ].Name },
				{ ShortcutScriptGenerationSyntax.TemplateFindLastNameString, entry.LastIdentity.Name },
				{ ShortcutScriptGenerationSyntax.TemplateFindLastTagString, entry.LastIdentity.Tag },
				{ ShortcutScriptGenerationSyntax.TemplateFindPronounString, entry.Pronoun },
				{ ShortcutScriptGenerationSyntax.TemplateFindTagString, entry.Identities[ 0 ].Tag },
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
