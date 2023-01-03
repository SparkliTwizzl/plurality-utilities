namespace PluralityUtilities.AutoHotkeyScripts.Containers
{
	public class Person
	{
		public List<Identity> Identities { get; set; } = new List<Identity>();
		public string Pronoun { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;


		public static bool operator ==(Person left, Person right)
		{
			return left.Identities.SequenceEqual(right.Identities) && left.Pronoun.Equals(right.Pronoun) && left.Decoration.Equals(right.Decoration);
		}

		public static bool operator !=(Person left, Person right)
		{
			return !left.Identities.SequenceEqual(right.Identities) || !left.Pronoun.Equals(right.Pronoun) || !left.Decoration.Equals(right.Decoration);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return this == (Person)obj;
		}

		public override int GetHashCode()
		{
			return Identities.GetHashCode() ^ Pronoun.GetHashCode() ^ Decoration.GetHashCode();
		}
	}
}
