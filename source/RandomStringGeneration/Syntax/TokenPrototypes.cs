using Petrichor.Common.Containers;

namespace Petrichor.RandomStringGeneration.Syntax
{
	/// <summary>
	/// Represents a collection of data token prototypes used in random string generation.
	/// </summary>
	public static class TokenPrototypes
	{
		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.AllowedCharacters"/> option.
		/// </summary>
		public static DataToken AllowedCharacters => new()
		{
			Key = "allowed-characters",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.StringCount"/> option.
		/// </summary>
		public static DataToken StringCount => new()
		{
			Key = "string-count",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.StringLength"/> option.
		/// </summary>
		public static DataToken StringLength => new()
		{
			Key = "string-length",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Maps token keys to token prototypes.
		/// Allows mapping command options to token prototypes.
		/// </summary>
		public static Dictionary<string, DataToken> CommandTokenMap => new()
		{
			{ AllowedCharacters.Key, AllowedCharacters },
			{ StringCount.Key, StringCount },
			{ StringLength.Key, StringLength },
		};
	}
}
