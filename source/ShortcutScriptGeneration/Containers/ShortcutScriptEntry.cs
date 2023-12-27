namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ShortcutScriptEntry : IEquatable<ShortcutScriptEntry>
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


		public static bool operator ==( ShortcutScriptEntry a, ShortcutScriptEntry b ) => a.Equals( b );

		public static bool operator !=( ShortcutScriptEntry a, ShortcutScriptEntry b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ShortcutScriptEntry ) obj );
		}

		public bool Equals( ShortcutScriptEntry? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Identities.SequenceEqual( other.Identities ) && Pronoun.Equals( other.Pronoun ) && Decoration.Equals( other.Decoration );
		}

		public override int GetHashCode() => Identities.GetHashCode() ^ Pronoun.GetHashCode() ^ Decoration.GetHashCode();
	}
}
