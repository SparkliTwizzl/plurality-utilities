namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Represents a string with an associated line number.
	/// </summary>
	public sealed class IndexedString : IEquatable<IndexedString>
	{
		/// <summary>
		/// An empty instance of <see cref="IndexedString"/>.
		/// </summary>
		public static IndexedString Empty => new();

		/// <summary>
		/// The string value associated with this indexed string.
		/// </summary>
		public string Value { get; set; } = string.Empty;

		/// <summary>
		/// The line number associated with this indexed string.
		/// </summary>
		public int LineNumber { get; set; } = 0;


		/// <summary>
		/// Initializes a new instance of the <see cref="IndexedString"/> class.
		/// </summary>
		public IndexedString() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexedString"/> class with the specified parameters.
		/// </summary>
		/// <param name="value">The string value to associate with this indexed string.</param>
		public IndexedString(string value) => Value = value;

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexedString"/> class with the specified value and line number.
		/// </summary>
		/// <param name="value">The string value to associate with this indexed string.</param>
		/// <param name="lineNumber">The line number to associate with this indexed string.</param>
		public IndexedString(string value, int lineNumber)
		{
			Value = value;
			LineNumber = lineNumber;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexedString"/> class by copying the values from another instance.
		/// </summary>
		/// <param name="other">The <see cref="IndexedString"/> instance to copy values from.</param>
		public IndexedString(IndexedString other)
		{
			Value = other.Value;
			LineNumber = other.LineNumber;
		}


		/// <summary>
		/// Indexes a raw string by creating an array of <see cref="IndexedString"/> with the specified value.
		/// </summary>
		/// <param name="value">The raw string to index.</param>
		/// <returns>An array of <see cref="IndexedString"/> representing the indexed string.</returns>
		public static IndexedString[] IndexRawStrings(string value) => IndexRawStrings(new[] { value });

		/// <summary>
		/// Indexes an array of raw strings by creating an array of <see cref="IndexedString"/>.
		/// </summary>
		/// <param name="strings">An array of strings to index.</param>
		/// <returns>An array of <see cref="IndexedString"/> representing the indexed strings.</returns>
		public static IndexedString[] IndexRawStrings(string[] strings)
		{
			var result = new List<IndexedString>();
			for (var i = 0; i < strings.Length; ++i)
			{
				result.Add(new(strings[i], i + 1));
			}
			return result.ToArray();
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object? obj)
		{
			if (obj is null)
			{
				return false;
			}
			var other = obj as IndexedString;
			return Equals(other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="IndexedString"/> is equal to the current <see cref="IndexedString"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals(IndexedString? other)
		{
			if (other is null)
			{
				return false;
			}
			return Value.Equals(other.Value) && LineNumber.Equals(other.LineNumber);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Value.GetHashCode() ^ LineNumber.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString() => $"{Value} <LINE {LineNumber}>";

		/// <summary>
		/// Determines whether two specified instances of <see cref="IndexedString"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(IndexedString a, IndexedString b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="IndexedString"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(IndexedString a, IndexedString b) => !a.Equals(b);
	}
}
