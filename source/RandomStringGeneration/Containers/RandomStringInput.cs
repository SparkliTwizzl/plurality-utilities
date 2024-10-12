using Petrichor.RandomStringGeneration.Syntax;


namespace Petrichor.RandomStringGeneration.Containers
{
	/// <summary>
	/// Represents the input data for random string generation.
	/// </summary>
	public sealed class RandomStringInput : IEquatable<RandomStringInput>
	{
		/// <summary>
		/// Gets or sets the allowed characters for string generation.
		/// </summary>
		public string AllowedCharacters { get; set; } = Commands.Arguments.AllowedCharactersDefault;

		/// <summary>
		/// Gets or sets the number of strings to generate.
		/// </summary>
		public int StringCount { get; set; } = Commands.Arguments.StringCountDefault;

		/// <summary>
		/// Gets or sets the length of each generated string.
		/// </summary>
		public int StringLength { get; set; } = Commands.Arguments.StringLengthDefault;


		/// <summary>
		/// Initializes a new instance of the <see cref="RandomStringInput"/> class.
		/// </summary>
		public RandomStringInput() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomStringInput"/> class with specified parameters.
		/// </summary>
		/// <param name="allowedCharacters">The allowed characters for string generation.</param>
		/// <param name="stringCount">The number of strings to generate.</param>
		/// <param name="stringLength">The length of each generated string.</param>
		public RandomStringInput(string allowedCharacters, int stringCount, int stringLength)
		{
			AllowedCharacters = allowedCharacters;
			StringCount = stringCount;
			StringLength = stringLength;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomStringInput"/> class by copying another instance.
		/// </summary>
		/// <param name="other">The instance to copy.</param>
		public RandomStringInput(RandomStringInput other)
		{
			AllowedCharacters = other.AllowedCharacters;
			StringCount = other.StringCount;
			StringLength = other.StringLength;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object? obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return Equals((RandomStringInput)obj);
		}

		/// <summary>
		/// Determines whether the specified <see cref="RandomStringInput"/> is equal to the current <see cref="RandomStringInput"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals(RandomStringInput? other)
		{
			if (other is null)
			{
				return false;
			}
			return AllowedCharacters.Equals(other.AllowedCharacters) && StringCount.Equals(other.StringCount) && StringLength.Equals(other.StringLength);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => AllowedCharacters.GetHashCode() ^ StringCount.GetHashCode() ^ StringLength.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString() => $"{nameof(AllowedCharacters)}={AllowedCharacters}, {nameof(StringCount)}={StringCount}, {nameof(StringLength)}={StringLength}";

		/// <summary>
		/// Determines whether two specified instances of <see cref="RandomStringInput"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(RandomStringInput a, RandomStringInput b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="RandomStringInput"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(RandomStringInput a, RandomStringInput b) => !a.Equals(b);
	}
}
