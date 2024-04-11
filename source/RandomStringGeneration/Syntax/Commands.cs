namespace Petrichor.RandomStringGeneration.Syntax
{
	public readonly struct Commands
	{
		public const string ModuleCommand = "generateRandomStrings";


		public readonly struct Options
		{
			public const string AllowedCharacters = "--allowedCharacters";
			public const string StringCount = "--stringCount";
			public const string StringLength = "--stringLength";

			public const string AllowedCharactersDefaultValue = "abcdefghijklmnopqrstuvwxyz";
			public const int StringCountDefaultValue = 1;
			public const int StringLengthDefaultValue = 10;

			public static Dictionary<string, string> LookUpTable => new()
			{
				{ Tokens.AllowedCharacters.Key, AllowedCharacters },
				{ Tokens.StringCount.Key, StringCount },
				{ Tokens.StringLength.Key, StringLength },
			};
		}
	}
}
