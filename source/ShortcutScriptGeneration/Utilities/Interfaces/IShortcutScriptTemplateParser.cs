namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface IShortcutScriptTemplateParser
	{
		string[] ParseTemplatesFromData( string[] data, ref int i );
	}
}