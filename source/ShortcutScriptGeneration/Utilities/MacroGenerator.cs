using Petrichor.Common.Utilities;
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


		private static string ApplyFindAndReplaceToTemplate( string templateString, Dictionary<string, string> findAndReplace, Dictionary<string, string> fields )
		{
			var macro = templateString;
			foreach ( var findTag in TemplateFindTags.LookUpTable )
			{
				macro = macro.Replace( $"{findTag}", fields[ findTag ] );
			}
			foreach ( var pair in findAndReplace )
			{
				macro = macro.Replace( pair.Key, pair.Value );
			}
			return macro
					.Replace( Common.Syntax.ControlSequences.EscapeStandin, Common.Syntax.ControlSequences.Escape.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagOpenStandin, Common.Syntax.ControlSequences.FindTagOpen.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagCloseStandin, Common.Syntax.ControlSequences.FindTagClose.ToString() );
		}

		private static string ApplyTextCaseToMacro( string macro, string textCase ) => textCase switch
		{
			TemplateTextCases.FirstCaps => macro.ToFirstCaps(),
			TemplateTextCases.Lower => macro.ToLower(),
			TemplateTextCases.Unchanged => macro,
			TemplateTextCases.Upper => macro.ToUpper(),
			_ => macro,
		};

		private static string ConvertTemplateToAutoHotkeySyntax( ScriptMacroTemplate template ) => $"::{template.TemplateFindString}::{template.TemplateReplaceString}";

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

			var macroFindString = ApplyFindAndReplaceToTemplate( template.TemplateFindString, template.FindAndReplace, fields );
			var macroReplaceString = ApplyFindAndReplaceToTemplate( template.TemplateReplaceString, template.FindAndReplace, fields );

			template.TemplateFindString = macroFindString;
			template.TemplateReplaceString = ApplyTextCaseToMacro( macroReplaceString, template.TextCase );

			return ConvertTemplateToAutoHotkeySyntax( template );
		}
	}
}
