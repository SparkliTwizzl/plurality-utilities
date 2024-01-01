namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptEntry : IEquatable<ScriptEntry>
	{
		public List<ScriptIdentity> Identities { get; set; } = new List<ScriptIdentity>();
		public string Pronoun { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;


		public ScriptEntry() { }
		public ScriptEntry( List<ScriptIdentity> identities, string pronoun, string decoration )
		{
			Identities = identities;
			Pronoun = pronoun;
			Decoration = decoration;
		}


		public static bool operator ==( ScriptEntry a, ScriptEntry b ) => a.Equals( b );

		public static bool operator !=( ScriptEntry a, ScriptEntry b ) => !a.Equals( b );

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ScriptEntry ) obj );
		}

		public bool Equals( ScriptEntry? other )
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
