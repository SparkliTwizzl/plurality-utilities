using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


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


		private static List<string> GenerateMacrosFromEntries( ScriptMacroTemplate[] templates, ScriptEntry entry )
		{
			var macros = new List<string>();
			foreach ( var identity in entry.Identities.ToList() )
			{
				var batch = new ScriptEntry( entry );
				batch.Identities.Clear();
				batch.Identities.Add( identity );
				macros.AddRange( GenerateMacrosFromIdentity( templates, batch ) );
			}
			return macros;
		}

		private static List<string> GenerateMacrosFromIdentity( ScriptMacroTemplate[] templates, ScriptEntry entry )
		{
			var macros = new List<string>();
			foreach ( var template in templates )
			{
				macros.Add( GenerateMacroFromTemplate( template, entry ) );
			}
			return macros;
		}

		private static string GenerateMacroFromTemplate( ScriptMacroTemplate template, ScriptEntry entry )
		{
			var macro = template.TemplateString;
			var fields = new Dictionary<string, string>()
			{
				{ TemplateFindTags.Color, entry.Color },
				{ TemplateFindTags.Decoration, entry.Decoration },
				{ TemplateFindTags.ID, entry.ID },
				{ TemplateFindTags.Name, entry.Identities[ 0 ].Name },
				{ TemplateFindTags.LastName, entry.LastIdentity.Name },
				{ TemplateFindTags.LastTag, entry.LastIdentity.Tag },
				{ TemplateFindTags.Pronoun, entry.Pronoun },
				{ TemplateFindTags.Tag, entry.Identities[ 0 ].Tag },
			};

			foreach ( var findTag in TemplateFindTags.LookUpTable )
			{
				macro = macro.Replace( $"{findTag}", fields[ findTag ] );
			}

			foreach ( var findAndReplace in template.FindAndReplace )
			{
				macro = macro.Replace( findAndReplace.Key, findAndReplace.Value );
			}

			return macro
					.Replace( Common.Syntax.ControlSequences.EscapeStandin, Common.Syntax.ControlSequences.Escape.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagOpenStandin, Common.Syntax.ControlSequences.FindTagOpen.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagCloseStandin, Common.Syntax.ControlSequences.FindTagClose.ToString() );
		}
	}
}
