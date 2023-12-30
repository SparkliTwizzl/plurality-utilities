namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface ITemplatesRegionParser
	{
		string[] ParseTemplatesFromData( string[] data, ref int i );
	}
}
