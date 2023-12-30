using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntriesRegionParser
	{
		ShortcutScriptEntry[] ParseEntriesFromData( string[] data, ref int i );
	}
}
