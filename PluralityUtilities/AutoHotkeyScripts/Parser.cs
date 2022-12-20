namespace PluralityUtilities.AutoHotkeyScripts
{
	public class Parser
	{
		public List<Person> People { get; private set; } = new List<Person>();


		public void ParseFile(string filePath)
		{
			var inputData = File.ReadAllLines(filePath);

			throw new NotImplementedException();
		}
	}
}