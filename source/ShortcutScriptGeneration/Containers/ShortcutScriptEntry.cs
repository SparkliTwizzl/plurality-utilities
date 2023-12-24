namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptEntry
	{
		public List<ShortcutScriptIdentity> Identities { get; set; } = new List<ShortcutScriptIdentity>();
		public string Pronoun { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;


		public ShortcutScriptEntry() { }
		public ShortcutScriptEntry( List<ShortcutScriptIdentity> identities, string pronoun, string decoration )
		{
			Identities = identities;
			Pronoun = pronoun;
			Decoration = decoration;
		}


		public static bool operator ==( ShortcutScriptEntry left, ShortcutScriptEntry right )
		{
			return left.Identities.SequenceEqual( right.Identities ) && left.Pronoun.Equals( right.Pronoun ) && left.Decoration.Equals( right.Decoration );
		}

		public static bool operator !=( ShortcutScriptEntry left, ShortcutScriptEntry right )
		{
			return !left.Identities.SequenceEqual( right.Identities ) || !left.Pronoun.Equals( right.Pronoun ) || !left.Decoration.Equals( right.Decoration );
		}

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( ShortcutScriptEntry )obj;
		}

		public override int GetHashCode()
		{
			return Identities.GetHashCode() ^ Pronoun.GetHashCode() ^ Decoration.GetHashCode();
		}
	}
}
