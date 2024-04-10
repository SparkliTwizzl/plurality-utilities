namespace Petrichor.RandomStringGeneration.Syntax
{
	public readonly struct Commands
	{
		public const string ModuleCommand = "generateRandomStrings";

		public const string AllowedCharactersOption = "--allowedCharacters";
		public const string StringCountOption = "--stringCount";
		public const string StringLengthOption = "--stringLength";

		public const string AllowedCharactersDefaultValue = "abcdefghijklmnopqrstuvwxyz";
		public const int StringCountDefaultValue = 1;
		public const int StringLengthDefaultValue = 10;


		public static Dictionary<string, string> LookUpTable => new()
		{
			{ Tokens.AllowedCharacters.Key, AllowedCharactersOption },
			{ Tokens.StringCount.Key, StringCountOption },
			{ Tokens.StringLength.Key, StringLengthOption },
		};
	}
}
