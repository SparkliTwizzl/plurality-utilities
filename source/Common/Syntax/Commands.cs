namespace Petrichor.Common.Syntax
{
	public readonly struct Commands
	{
		public const string None = "none";
		public const string Some = "some";

		public const string InputFileOption = "--inputFile";
		public const string LogModeOptionValueAll = "all";
		public const string LogModeOptionValueConsoleOnly = "consoleOnly";
		public const string LogModeOptionValueFileOnly = "fileOnly";
		public const string LogFileOption = "--logFile";
		public const string LogModeOption = "--logMode";
		public const string OutputFileOption = "--outputFile";


		public static Dictionary<string, string> LookUpTable => new()
		{
			{ Tokens.InputFile.Key, InputFileOption },
			{ Tokens.LogFile.Key, LogFileOption },
			{ Tokens.LogMode.Key, LogModeOption },
			{ Tokens.OutputFile.Key, OutputFileOption },
		};
	}
}
