namespace Petrichor.ShortcutScriptGeneration.Containers
{
	/// <summary>
	/// Represents an entry in the shortcut script generation process.
	/// </summary>
	public sealed class Entry : IEquatable<Entry>
	{
		/// <summary>
		/// Gets or sets the color associated with the entry.
		/// </summary>
		public string Color { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the decoration associated with the entry.
		/// </summary>
		public string Decoration { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the ID of the entry.
		/// </summary>
		public string ID { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the list of names associated with the entry.
		/// </summary>
		public List<Name> Names { get; set; } = new();

		/// <summary>
		/// Gets or sets the last name associated with the entry.
		/// </summary>
		public Name LastName { get; set; } = new();

		/// <summary>
		/// Gets or sets the pronoun associated with the entry.
		/// </summary>
		public string Pronoun { get; set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="Entry"/> class.
		/// </summary>
		public Entry() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Entry"/> class by copying another instance.
		/// </summary>
		/// <param name="other">The instance to copy.</param>
		public Entry(Entry other)
		{
			Color = other.Color;
			Decoration = other.Decoration;
			ID = other.ID;
			Names = other.Names;
			LastName = other.LastName;
			Pronoun = other.Pronoun;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Entry"/> class with the specified ID and names.
		/// </summary>
		/// <param name="id">The ID of the entry.</param>
		/// <param name="names">The list of names associated with the entry.</param>
		public Entry(string id, List<Name> names)
		{
			ID = id;
			Names = names;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Entry"/> class with the specified parameters.
		/// </summary>
		/// <param name="id">The ID of the entry.</param>
		/// <param name="names">The list of names associated with the entry.</param>
		/// <param name="lastName">The last name associated with the entry.</param>
		/// <param name="pronoun">The pronoun associated with the entry.</param>
		/// <param name="color">The color associated with the entry.</param>
		/// <param name="decoration">The decoration associated with the entry.</param>
		public Entry(string id, List<Name> names, Name lastName, string pronoun, string color, string decoration)
		{
			Color = color;
			Decoration = decoration;
			ID = id;
			Names = names;
			LastName = lastName;
			Pronoun = pronoun;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object? obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return Equals((Entry)obj);
		}

		/// <summary>
		/// Determines whether the specified <see cref="Entry"/> is equal to the current instance.
		/// </summary>
		/// <param name="other">The <see cref="Entry"/> to compare with the current instance.</param>
		/// <returns><c>true</c> if the specified <see cref="Entry"/> is equal to the current instance; otherwise, <c>false</c>.</returns>
		public bool Equals(Entry? other)
		{
			if (other is null)
			{
				return false;
			}
			return Color.Equals(other.Color) && Decoration.Equals(other.Decoration) && ID.Equals(other.ID) && Names.SequenceEqual(other.Names) && LastName.Equals(other.LastName) && Pronoun.Equals(other.Pronoun);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current instance.</returns>
		public override int GetHashCode() => Color.GetHashCode() ^ Decoration.GetHashCode() ^ ID.GetHashCode() ^ Names.GetHashCode() ^ LastName.GetHashCode() ^ Pronoun.GetHashCode();

		/// <summary>
		/// Determines whether two <see cref="Entry"/> instances are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Entry a, Entry b) => a.Equals(b);

		/// <summary>
		/// Determines whether two <see cref="Entry"/> instances are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Entry a, Entry b) => !a.Equals(b);

	}
}
