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

			public static Dictionary<string, string> LookUpTable => new()
			{
				{ Tokens.AllowedCharacters.Key, AllowedCharacters },
				{ Tokens.StringCount.Key, StringCount },
				{ Tokens.StringLength.Key, StringLength },
			};


			public readonly struct Defaults
			{
				public const string AllowedCharactersValue = "abcdefghijklmnopqrstuvwxyz";
				public const int StringCountValue = 1;
				public const int StringLengthValue = 10;
			}
		}
	}
}
