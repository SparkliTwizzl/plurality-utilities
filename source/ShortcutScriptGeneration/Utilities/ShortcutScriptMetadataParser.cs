using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMetadataParser : IShortcutScriptMetadataParser
	{
		private const string DefaultIconFilePathToken = "default-icon:";
		private StringTokenParser TokenParser = new StringTokenParser();


		public ShortcutScriptMetadata ParseMetadataFromData(string[] data, ref int i)
		{
			throw new NotImplementedException();
		}
	}
}