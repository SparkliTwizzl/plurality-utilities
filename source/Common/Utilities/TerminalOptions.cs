using Petrichor.Common.Syntax;
using System.CommandLine;


namespace Petrichor.Common.Utilities
{
	public static class TerminalOptions
	{
		public static string DefaultInputDirectory => Info.ProjectDirectories.InputDirectory;
		public static string DefaultInputFileName => "input.petrichor";
		public static string DefaultLogDirectory => Info.ProjectDirectories.LogDirectory;
		public static string DefaultLogFileName => $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fffffff}.log";
		public static string DefaultOutputDirectory => Info.ProjectDirectories.OutputDirectory;
		public static string DefaultOutputFileName => "output.txt";

		public static bool IsAutoExitEnabled { get; set; } = false;


		public static Option<bool> AutoExit { get; } = new Option<bool>(
			name: Commands.Options.AutoExit,
			description: "Exit without waiting for user input.",
			getDefaultValue: () => false);

		public static Option<string> InputFile { get; } = new(
			name: Commands.Options.InputFile,
			description: "Path to input file. If not provided, a default file path will be used.",
			getDefaultValue: () => Path.Combine( DefaultInputDirectory, DefaultInputFileName ) );

		public static Option<string> LogMode { get; } = new Option<string>(
			name: Commands.Options.LogMode,
			description: "Logging mode to enable. See documentation for available modes.",
			getDefaultValue: () => Commands.Options.LogModeValueAll )
			.FromAmong(
				Commands.Options.LogModeValueAll,
				Commands.Options.LogModeValueConsoleOnly,
				Commands.Options.LogModeValueFileOnly,
				Commands.Options.LogModeValueNone );

		public static Option<string> LogFile { get; } = new(
			name: Commands.Options.LogFile,
			description: "Path to generate log file at. If not provided, a default filepath will be used.",
			getDefaultValue: () => Path.Combine( DefaultLogDirectory, DefaultLogFileName ) );

		public static Option<string> OutputFile { get; } = new(
				name: Commands.Options.OutputFile,
				description: "Path to generate output file at. If not provided, a default file path will be used.",
				getDefaultValue: () => Path.Combine( DefaultOutputDirectory, DefaultOutputFileName ) );
	}
}
