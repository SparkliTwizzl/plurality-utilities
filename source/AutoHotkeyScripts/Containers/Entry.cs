namespace Petrichor.AutoHotkeyScripts.Containers
{
	public class Entry
	{
		public List<Identity> Identities { get; set; } = new List<Identity>();
		public string Pronoun { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;


		public Entry() { }
		public Entry( List<Identity> identities, string pronoun, string decoration )
		{
			Identities = identities;
			Pronoun = pronoun;
			Decoration = decoration;
		}


		public static bool operator ==( Entry left, Entry right )
		{
			return left.Identities.SequenceEqual( right.Identities ) && left.Pronoun.Equals( right.Pronoun ) && left.Decoration.Equals( right.Decoration );
		}

		public static bool operator !=( Entry left, Entry right )
		{
			return !left.Identities.SequenceEqual( right.Identities ) || !left.Pronoun.Equals( right.Pronoun ) || !left.Decoration.Equals( right.Decoration );
		}

		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( Entry )obj;
		}

		public override int GetHashCode()
		{
			return Identities.GetHashCode() ^ Pronoun.GetHashCode() ^ Decoration.GetHashCode();
		}
	}
}
