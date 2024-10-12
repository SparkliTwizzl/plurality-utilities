namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class Name : IEquatable<Name>
	{
		/// <summary>
		/// An empty instance of <see cref="Name"/>.
		/// </summary>
		public static Name Empty => new();

		/// <summary>
		/// The string value associated with this name.
		/// </summary>
		public string Value { get; set; } = string.Empty;

		/// <summary>
		/// The tag associated with this name.
		/// </summary>
		public string Tag { get; set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="Name"/> class.
		/// </summary>
		public Name() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Name"/> class with the specified parameters.
		/// </summary>
		/// <param name="value">The value to associated with this name.</param>
		/// <param name="tag">The tag to associate with this name.</param>
		public Name( string value, string tag )
		{
			Value = value;
			Tag = tag;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( Name ) obj );
		}

		/// <summary>
		/// Determines whether the specified <see cref="Name"/> is equal to the current <see cref="Name"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals( Name? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Value.Equals( other.Value ) && Tag.Equals( other.Tag );
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Value.GetHashCode() ^ Tag.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString() => $"{nameof(Value)}={Value},{nameof(Tag)}={Tag}";

		/// <summary>
		/// Determines whether two specified instances of <see cref="Name"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(Name a, Name b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="Name"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(Name a, Name b) => !a.Equals(b);
	}
}
