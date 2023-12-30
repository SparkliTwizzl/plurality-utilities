using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IModuleOptionsRegionParser
	{
		ShortcutScriptModuleOptions ParseModuleOptionsFromData( string[] data, ref int i );
	}
}
