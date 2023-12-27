using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptEntryParser
	{
		/// <summary>
		/// throws BlankInputFieldException if file contains a field with no value<para/>
		/// throws DuplicateInputFieldException if file contains an entry with more than one decoration field<para/>
		/// throws DuplicateInputFieldException if file contains an entry with more than one pronoun field<para/>
		/// throws InputEntryNotClosedException if file contains an entry that is not closed<para/>
		/// throws FileNotFoundException if file data could not be read<para/>
		/// throws InvalidInputFieldException if file contains a tag field with spaces in it<para/>
		/// throws MissingInputFieldException if file contains an entry with no identity fields<para/>
		/// throws MissingInputFieldException if file contains an identity field with no name field<para/>
		/// throws MissingInputFieldException if file contains an identity field with no tag field<para/>
		/// throws UnexpectedCharacterException if file contains a line that starts with an unexpected character<para/>
		/// </summary>
		/// <param name="data">input data read from file</param>
		/// <param name="i">index of first open bracket of entries region in the input data</param>
		/// <returns>parsed entries</returns>
		ShortcutScriptEntry[] ParseEntriesFromData( string[] data, ref int i );
	}
}