using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntryParser : IRegionParser<ScriptEntry>
	{
		new ScriptEntry Parse( string[] regionData );
	}
}
