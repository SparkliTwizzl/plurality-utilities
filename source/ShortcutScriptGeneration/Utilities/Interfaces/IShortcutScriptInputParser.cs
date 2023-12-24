using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptInputParser
	{
		ShortcutScriptInput ParseInputFile(string inputFilePath);
	}
}