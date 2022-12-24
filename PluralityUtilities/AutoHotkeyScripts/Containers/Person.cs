namespace PluralityUtilities.AutoHotkeyScripts.Containers
{
	public class Person
	{
		public List<Identity> Identities { get; set; } = new List<Identity>();
		public List<string> Names { get; set; } = new List<string>();
		public string Pronoun { get; set; } = string.Empty;
		public List<string> Tags { get; set; } = new List<string>();
	}
}
