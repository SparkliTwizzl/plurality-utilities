using Petrichor.Common.Containers;


namespace Petrichor.RandomStringGeneration.Syntax
{
	public readonly struct Tokens
	{
		public static DataToken AllowedCharacters => new()
		{
			Key = "allowed-characters",
			MaxAllowed = 1,
		};
		public static DataToken StringCount => new()
		{
			Key = "string-count",
			MaxAllowed = 1,
		};
		public static DataToken StringLength => new()
		{
			Key = "string-length",
			MaxAllowed = 1,
		};


		public static Dictionary<string, DataToken> CommandOptionLookUpTable => new()
		{
			{ AllowedCharacters.Key, AllowedCharacters },
			{ StringCount.Key, StringCount },
			{ StringLength.Key, StringLength },
		};

		public static Dictionary<string, DataToken> LookUpTable => new()
		{
			{ AllowedCharacters.Key, AllowedCharacters },
			{ StringCount.Key, StringCount },
			{ StringLength.Key, StringLength },
		};
	}
}
