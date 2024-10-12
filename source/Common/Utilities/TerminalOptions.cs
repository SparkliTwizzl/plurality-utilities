using Petrichor.Common.Syntax;
using System.CommandLine;

namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Provides terminal options and default file paths for input, output, and logging.
	/// </summary>
	public static class TerminalOptions
	{
		/// <summary>
		/// Gets the default input directory path.
		/// </summary>
		public static string DefaultInputDirectory => Info.ProjectDirectories.InputDirectory;

		/// <summary>
		/// Gets the default input file name.
		/// </summary>
		public static string DefaultInputFileName => "input.petrichor";

		/// <summary>
		/// Gets the default log directory path.
		/// </summary>
		public static string DefaultLogDirectory => Info.ProjectDirectories.LogDirectory;

		/// <summary>
		/// Gets the default log file name with the current timestamp.
		/// </summary>
		public static string DefaultLogFileName => $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fffffff}.log";

		/// <summary>
		/// Gets the default output directory path.
		/// </summary>
		public static string DefaultOutputDirectory => Info.ProjectDirectories.OutputDirectory;

		/// <summary>
		/// Gets the default output file name.
		/// </summary>
		public static string DefaultOutputFileName => "output.txt";

		/// <summary>
		/// Gets or sets a value indicating whether auto-exit is enabled.
		/// </summary>
		public static bool IsAutoExitEnabled { get; set; } = false;

		/// <summary>
		/// Gets a command-line option to enable or disable auto-exit.
		/// </summary>
		public static Option<bool> AutoExit { get; } = new Option<bool>(
			name: Commands.Parameters.AutoExit,
			description: "Exit without waiting for user input.",
			getDefaultValue: () => false);

		/// <summary>
		/// Gets a command-line option for specifying the input file path.
		/// Uses a default file path if not provided.
		/// </summary>
		public static Option<string> InputFile { get; } = new(
			name: Commands.Parameters.InputFile,
			description: "Path to input file. If not provided, a default file path will be used.",
			getDefaultValue: () => Path.Combine(DefaultInputDirectory, DefaultInputFileName));

		/// <summary>
		/// Gets a command-line option for setting the logging mode.
		/// </summary>
		public static Option<string> LogMode { get; } = new Option<string>(
			name: Commands.Parameters.LogMode,
			description: "Logging mode to enable. See documentation for available modes.",
			getDefaultValue: () => Commands.Arguments.LogModeDefault)
			.FromAmong(
				Commands.Arguments.LogModeAll,
				Commands.Arguments.LogModeConsoleOnly,
				Commands.Arguments.LogModeFileOnly,
				Commands.Arguments.LogModeNone);

		/// <summary>
		/// Gets a command-line option for specifying the log file path.
		/// Uses a default file path if not provided.
		/// </summary>
		public static Option<string> LogFile { get; } = new(
			name: Commands.Parameters.LogFile,
			description: "Path to generate log file at. If not provided, a default filepath will be used.",
			getDefaultValue: () => Path.Combine(DefaultLogDirectory, DefaultLogFileName));

		/// <summary>
		/// Gets a command-line option for specifying the output file path.
		/// Uses a default file path if not provided.
		/// </summary>
		public static Option<string> OutputFile { get; } = new(
				name: Commands.Parameters.OutputFile,
				description: "Path to generate output file at. If not provided, a default file path will be used.",
				getDefaultValue: () => Path.Combine(DefaultOutputDirectory, DefaultOutputFileName));
	}
}
