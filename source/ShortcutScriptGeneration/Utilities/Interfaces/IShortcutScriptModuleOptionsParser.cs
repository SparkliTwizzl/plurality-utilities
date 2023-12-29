using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptModuleOptionsParser
	{
		ShortcutScriptModuleOptions ParseModuleOptionsFromData( string[] data, ref int i );
	}
}