using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IMacroGenerator
	{
		string[] Generate( ScriptInput input );
	}
}