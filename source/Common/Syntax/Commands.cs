namespace Petrichor.Common.Syntax
{
	public readonly struct Commands
	{
		public const string None = "none";
		public const string Some = "some";


		public readonly struct Options
		{
			public const string AutoExit = "--autoExit";
			public const string InputFile = "--inputFile";
			public const string LogFile = "--logFile";
			public const string LogMode = "--logMode";
			public const string OutputFile = "--outputFile";

			public const string LogModeValueAll = "all";
			public const string LogModeValueConsoleOnly = "consoleOnly";
			public const string LogModeValueNone = "none";
			public const string LogModeValueFileOnly = "fileOnly";

			public static Dictionary<string, string> LookUpTable => new()
			{
				{ Tokens.InputFile.Key, InputFile },
				{ Tokens.LogFile.Key, LogFile },
				{ Tokens.LogMode.Key, LogMode },
				{ Tokens.OutputFile.Key, OutputFile },
			};
		}
	}
}
