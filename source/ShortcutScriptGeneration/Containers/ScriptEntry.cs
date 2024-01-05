namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ScriptEntry : IEquatable<ScriptEntry>
	{
		public string Color { get; set; } = string.Empty;
		public string Decoration { get; set; } = string.Empty;
		public string ID { get; set; } = string.Empty;
		public List<ScriptIdentity> Identities { get; set; } = new();
		public ScriptIdentity LastIdentity { get; set; } = new();
		public string Pronoun { get; set; } = string.Empty;


		public ScriptEntry() { }
		public ScriptEntry( string id, List<ScriptIdentity> identities )
		{
			ID = id;
			Identities = identities;
		}
		public ScriptEntry( string id, List<ScriptIdentity> identities, ScriptIdentity lastIdentity, string pronoun, string color, string decoration )
		{
			Color = color;
			Decoration = decoration;
			ID = id;
			Identities = identities;
			LastIdentity = lastIdentity;
			Pronoun = pronoun;
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
			return Color.Equals( other.Color ) && Decoration.Equals( other.Decoration ) && ID.Equals( other.ID ) && Identities.SequenceEqual( other.Identities ) && LastIdentity.Equals( other.LastIdentity ) && Pronoun.Equals( other.Pronoun );
		}

		public override int GetHashCode() => Color.GetHashCode() ^ Decoration.GetHashCode() ^ ID.GetHashCode() ^ Identities.GetHashCode() ^ LastIdentity.GetHashCode() ^ Pronoun.GetHashCode();
	}
}
