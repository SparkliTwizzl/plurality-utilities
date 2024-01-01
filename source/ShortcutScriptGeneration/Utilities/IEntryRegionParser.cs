using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IEntryRegionParser : IRegionParser<ScriptEntry>
	{
		new ScriptEntry Parse( string[] regionData );
	}
}
