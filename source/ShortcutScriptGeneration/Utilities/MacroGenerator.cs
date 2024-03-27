using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
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


		private static List<string> GenerateMacrosFromEntries( string[] templates, ScriptEntry entry )
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
				{ TemplateFindStrings.Color, entry.Color },
				{ TemplateFindStrings.Decoration, entry.Decoration },
				{ TemplateFindStrings.ID, entry.ID },
				{ TemplateFindStrings.Name, entry.Identities[ 0 ].Name },
				{ TemplateFindStrings.LastName, entry.LastIdentity.Name },
				{ TemplateFindStrings.LastTag, entry.LastIdentity.Tag },
				{ TemplateFindStrings.Pronoun, entry.Pronoun },
				{ TemplateFindStrings.Tag, entry.Identities[ 0 ].Tag },
			 };
			foreach ( var findString in ScriptTemplateFindStrings.LookUpTable )
			{
				macro = macro
					.Replace( $"{findString}", fields[ findString ] )
					.Replace( Common.Syntax.ControlSequences.EscapeStandin, Common.Syntax.ControlSequences.Escape.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagOpenStandin, Common.Syntax.ControlSequences.FindTagOpen.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagCloseStandin, Common.Syntax.ControlSequences.FindTagClose.ToString() );
			}
			return macro;
		}
	}
}
