using Petrichor.Common.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface ITemplatesRegionParser : IRegionParser<string[]>
	{
		new string[] Parse( string[] regionData, ref int i );
	}
}
