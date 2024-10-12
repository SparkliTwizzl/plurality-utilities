using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutProcessor : IShortcutProcessor
	{
		/// <summary>
		/// Generates shortcuts from shortcut templates and entries.
		/// Adds generated shortcuts to the existing shortcut list.
		/// </summary>
		/// <param name="input">The input data to modify.</param>
		/// <returns>The modified <see cref="ShortcutScriptInput"/> instance.</returns>
		public ShortcutScriptInput GenerateTemplatedShortcuts(ShortcutScriptInput input)
		{
			var shortcuts = input.Shortcuts.ToList();
			foreach ( var entry in input.Entries )
			{
				shortcuts.AddRange( GenerateTemplatedShortcutListFromEntries( input.ShortcutTemplates, entry ) );
			}
			input.Shortcuts = shortcuts.ToArray();
			return input;			
		}

		/// <summary>
		/// Sanitizes plaintext shortcuts.
		/// Overwrites the existing shortcut list.
		/// </summary>
		/// <param name="input">The input data to modify.</param>
		/// <returns>The modified <see cref="ShortcutScriptInput"/> instance.</returns>
		public ShortcutScriptInput SanitizePlaintextShortcuts(ShortcutScriptInput input)
		{
			var shortcuts = new List<string>();
			foreach ( var rawShortcut in input.Shortcuts )
			{
				shortcuts.Add( ConvertPlaintextShortcutToAutoHotkeySyntax( rawShortcut ).CodepointsToChars() );
			}
			input.Shortcuts = shortcuts.ToArray();
			return input;
		}


		private static string ApplyEntryFieldsToTemplateString( string templateString, Dictionary<string, string> fields )
		{
			var macro = templateString;
			foreach ( var findTag in TemplateFindTags.LookupTable )
			{
				macro = macro.Replace( $"{findTag}", fields[ findTag ] );
			}
			return macro;
		}

		private static string ApplyFindAndReplacePairsToTemplateString( string templateString, Dictionary<string, string> findAndReplace )
		{
			var macro = templateString;
			foreach ( var pair in findAndReplace )
			{
				macro = macro.Replace( pair.Key, pair.Value );
			}
			return macro;
		}

		private static string ApplyTextCaseToString( string text, string textCase ) => textCase switch
		{
			TemplateTextCases.FirstCaps => text.ToFirstCaps(),
			TemplateTextCases.Lower => text.ToLower(),
			TemplateTextCases.Unchanged => text,
			TemplateTextCases.Upper => text.ToUpper(),
			_ => text,
		};

		private static string ConvertPlaintextShortcutToAutoHotkeySyntax( string shortcut )
		{
			var parts = shortcut.Split( ControlSequences.TemplateFindStringDelimiter );
			var find = parts[ 0 ].Trim();
			var replace = parts[ 1 ].Trim();
			return $"::{find}::{replace}";
		}

		private static string ConvertShortcutTemplateToAutoHotkeySyntax( TextShortcut template ) => $"::{template.TemplateFindString}::{template.TemplateReplaceString}";

		private static string GenerateTemplatedShortcutFromName( TextShortcut template, Name name, Entry entry )
		{
			var fields = new Dictionary<string, string>()
			{
				{ TemplateFindTags.Color, entry.Color },
				{ TemplateFindTags.Decoration, entry.Decoration },
				{ TemplateFindTags.ID, entry.ID },
				{ TemplateFindTags.Name, name.Value },
				{ TemplateFindTags.LastName, entry.LastName.Value },
				{ TemplateFindTags.LastTag, entry.LastName.Tag },
				{ TemplateFindTags.Pronoun, entry.Pronoun },
				{ TemplateFindTags.Tag, name.Tag },
			};

			var macroFindString = ApplyEntryFieldsToTemplateString( template.TemplateFindString, fields );
			macroFindString = ReplaceStandinSequencesInMacro( macroFindString );

			var macroReplaceString = ApplyEntryFieldsToTemplateString( template.TemplateReplaceString, fields );
			macroReplaceString = ApplyFindAndReplacePairsToTemplateString( macroReplaceString, template.FindAndReplace );
			macroReplaceString = ReplaceStandinSequencesInMacro( macroReplaceString );
			macroReplaceString = ApplyTextCaseToString( macroReplaceString, template.TextCase );

			var modifiedTemplate = new TextShortcut()
			{
				TemplateFindString = macroFindString,
				TemplateReplaceString = macroReplaceString,
			};
			return ConvertShortcutTemplateToAutoHotkeySyntax( modifiedTemplate );
		}

		private static List<string> GenerateTemplatedShortcutListFromEntries( TextShortcut[] templates, Entry entry )
		{
			var shortcuts = new List<string>();
			foreach ( var name in entry.Names.ToList() )
			{
				shortcuts.AddRange( GenerateTemplatedShortcutListFromName( templates, name, entry ) );
			}
			return shortcuts;
		}

		private static List<string> GenerateTemplatedShortcutListFromName( TextShortcut[] templates, Name name, Entry entry )
		{
			var shortcuts = new List<string>();
			foreach ( var template in templates )
			{
				shortcuts.Add( GenerateTemplatedShortcutFromName( template, name, entry ) );
			}
			return shortcuts;
		}

		private static string ReplaceStandinSequencesInMacro( string macro ) => macro.CodepointsToChars();
	}
}
