using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IModuleOptionsRegionParser : IRegionParser<ScriptModuleOptions>
	{
		new ScriptModuleOptions Parse( string[] regionData, ref int i );
	}
}
