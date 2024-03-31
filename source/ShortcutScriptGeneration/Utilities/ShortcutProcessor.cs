using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutProcessor : IShortcutProcessor
	{
		public ScriptInput ProcessAndStoreShortcuts( ScriptInput input )
		{
			var shortcuts = new List<string>();
			foreach ( var rawShortcut in input.Shortcuts )
			{
				shortcuts.Add( ConvertShortcutToAutoHotkeySyntax( rawShortcut ) );
			}
			foreach ( var entry in input.Entries )
			{
				shortcuts.AddRange( GenerateTemplatedShortcutsFromEntries( input.ShortcutTemplates, entry ) );
			}
			input.Shortcuts = shortcuts.ToArray();
			return input;
		}


		private static string ApplyFindAndReplaceToTemplate( string templateString, Dictionary<string, string> findAndReplace, Dictionary<string, string> fields )
		{
			var shortcut = templateString;
			foreach ( var findTag in TemplateFindTags.LookUpTable )
			{
				shortcut = shortcut.Replace( $"{findTag}", fields[ findTag ] );
			}
			foreach ( var pair in findAndReplace )
			{
				shortcut = shortcut.Replace( pair.Key, pair.Value );
			}
			return shortcut
					.Replace( Common.Syntax.ControlSequences.EscapeStandin, Common.Syntax.ControlSequences.Escape.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagOpenStandin, Common.Syntax.ControlSequences.FindTagOpen.ToString() )
					.Replace( Common.Syntax.ControlSequences.FindTagCloseStandin, Common.Syntax.ControlSequences.FindTagClose.ToString() );
		}

		private static string ApplyTextCaseToString( string text, string textCase ) => textCase switch
		{
			TemplateTextCases.FirstCaps => text.ToFirstCaps(),
			TemplateTextCases.Lower => text.ToLower(),
			TemplateTextCases.Unchanged => text,
			TemplateTextCases.Upper => text.ToUpper(),
			_ => text,
		};

		private static string ConvertShortcutToAutoHotkeySyntax( string shortcut )
		{
			var parts = shortcut.Split( ControlSequences.ShortcutFindReplaceDivider );
			var find = parts[ 0 ].Trim();
			var replace = parts[ 1 ].Trim();
			return $"::{find}::{replace}";
		}

		private static string ConvertShortcutTemplateToAutoHotkeySyntax( ScriptShortcutData template ) => $"::{template.TemplateFindString}::{template.TemplateReplaceString}";

		private static List<string> GenerateTemplatedShortcutsFromEntries( ScriptShortcutData[] templates, ScriptEntry entry )
		{
			var shortcuts = new List<string>();
			foreach ( var identity in entry.Identities.ToList() )
			{
				shortcuts.AddRange( GenerateShortcutsFromIdentity( templates, identity, entry ) );
			}
			return shortcuts;
		}

		private static List<string> GenerateShortcutsFromIdentity( ScriptShortcutData[] templates, ScriptIdentity identity, ScriptEntry entry )
		{
			var shortcuts = new List<string>();
			foreach ( var template in templates )
			{
				shortcuts.Add( GenerateShortcutFromTemplate( template, identity, entry ) );
			}
			return shortcuts;
		}

		private static string GenerateShortcutFromTemplate( ScriptShortcutData template, ScriptIdentity identity, ScriptEntry entry )
		{
			var fields = new Dictionary<string, string>()
			{
				{ TemplateFindTags.Color, entry.Color },
				{ TemplateFindTags.Decoration, entry.Decoration },
				{ TemplateFindTags.ID, entry.ID },
				{ TemplateFindTags.Name, identity.Name },
				{ TemplateFindTags.LastName, entry.LastIdentity.Name },
				{ TemplateFindTags.LastTag, entry.LastIdentity.Tag },
				{ TemplateFindTags.Pronoun, entry.Pronoun },
				{ TemplateFindTags.Tag, identity.Tag },
			};

			var macroFindString = ApplyFindAndReplaceToTemplate( template.TemplateFindString, template.FindAndReplace, fields );
			var macroReplaceString = ApplyFindAndReplaceToTemplate( template.TemplateReplaceString, template.FindAndReplace, fields );

			var modifiedTemplate = new ScriptShortcutData
			{
				TemplateFindString = macroFindString,
				TemplateReplaceString = ApplyTextCaseToString( macroReplaceString, template.TextCase ),
			};
			return ConvertShortcutTemplateToAutoHotkeySyntax( modifiedTemplate );
		}
	}
}
