using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutProcessor
	{
		InputData ProcessAndStoreShortcuts( InputData input );
	}
}
