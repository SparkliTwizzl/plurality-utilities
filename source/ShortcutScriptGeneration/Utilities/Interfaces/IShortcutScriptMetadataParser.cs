using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptMetadataParser
	{
		ShortcutScriptMetadata ParseMetadataFromData(string[] data, ref int i);
	}
}