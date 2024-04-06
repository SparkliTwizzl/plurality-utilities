using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutProcessor
	{
		ScriptInput ProcessAndStoreShortcuts( ScriptInput input );
	}
}
