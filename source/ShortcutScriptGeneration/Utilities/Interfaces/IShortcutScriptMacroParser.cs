using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptMacroParser
	{
		string[] GenerateMacrosFromInput( ShortcutScriptInput input );
	}
}