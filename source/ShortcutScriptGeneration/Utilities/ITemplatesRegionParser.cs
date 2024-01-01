using Petrichor.Common.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public interface ITemplatesRegionParser : IRegionParser<string[]>
	{
		int TemplatesParsed { get; }


		new string[] Parse( string[] regionData );
	}
}
