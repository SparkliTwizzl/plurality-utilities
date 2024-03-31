using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutGenerator
	{
		ScriptInput GenerateAndStoreShortcuts( ScriptInput input );
	}
}
