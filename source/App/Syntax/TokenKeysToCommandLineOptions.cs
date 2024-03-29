using Petrichor.Common.Syntax;

namespace Petrichor.App.Syntax
{
	public readonly struct TokenKeysToCommandLineOptions
	{
		public static Dictionary<string, string> LookUpTable => new()
		{
			{ Tokens.InputFile.Key, CommandOptions.ShortcutScriptOptionInputFile },
			{ Tokens.LogFile.Key, CommandOptions.ShortcutScriptOptionLogFile },
			{ Tokens.LogMode.Key, CommandOptions.ShortcutScriptOptionLogMode },
			{ Tokens.OutputFile.Key, CommandOptions.ShortcutScriptOptionOutputFile },
		};
	}
}
