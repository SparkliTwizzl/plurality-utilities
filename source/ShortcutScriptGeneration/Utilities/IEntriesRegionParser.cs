using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntriesRegionParser : IRegionParser<ScriptEntry[]>
	{
		new ScriptEntry[] Parse( string[] regionData );
	}
}
