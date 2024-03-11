using Petrichor.Common.Info;
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
				{ ShortcutScriptSyntax.TemplateFindColorString, entry.Color },
				{ ShortcutScriptSyntax.TemplateFindDecorationString, entry.Decoration },
				{ ShortcutScriptSyntax.TemplateFindIDString, entry.ID },
				{ ShortcutScriptSyntax.TemplateFindNameString, entry.Identities[ 0 ].Name },
				{ ShortcutScriptSyntax.TemplateFindLastNameString, entry.LastIdentity.Name },
				{ ShortcutScriptSyntax.TemplateFindLastTagString, entry.LastIdentity.Tag },
				{ ShortcutScriptSyntax.TemplateFindPronounString, entry.Pronoun },
				{ ShortcutScriptSyntax.TemplateFindTagString, entry.Identities[ 0 ].Tag },
			 };
			foreach ( var findString in ScriptTemplateFindStrings.LookUpTable )
			{
				macro = macro
					.Replace( $"{ findString }", fields[ findString ] )
					.Replace( CommonSyntax.EscapeCharStandin, CommonSyntax.EscapeChar.ToString() )
					.Replace( CommonSyntax.FindTokenOpenCharStandin, CommonSyntax.FindTokenOpenChar.ToString() )
					.Replace( CommonSyntax.FindTokenCloseCharStandin, CommonSyntax.FindTokenCloseChar.ToString() );
			}
			return macro;
		}
	}
}
