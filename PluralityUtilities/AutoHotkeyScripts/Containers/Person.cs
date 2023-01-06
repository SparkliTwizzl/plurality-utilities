namespace PluralityUtilities.AutoHotkeyScripts.Containers
{
	public class Person
	{
		public List<Identity> Identities { get; set; } = new List<Identity>();
		public string Pronoun { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;
	}
}
