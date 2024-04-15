using Petrichor.Common.Syntax;
using System.CommandLine;


namespace Petrichor.Common.Utilities
{
	public readonly struct TerminalOptions
	{
		public static Option<string> InputFile { get; } = new(
			name: Commands.Options.InputFile,
			description: "Path to input file. If not provided, a default file path will be used." );

		public static Option<string> LogMode { get; } = new Option<string>(
			name: Commands.Options.LogMode,
			description: "Logging mode to enable. See documentation for available modes." )
			.FromAmong(
				Commands.Options.LogModeValueAll,
				Commands.Options.LogModeValueConsoleOnly,
				Commands.Options.LogModeValueFileOnly,
				Commands.Options.LogModeValueNone );

		public static Option<string> LogFile { get; } = new(
			name: Commands.Options.LogFile,
			description: "Path to generate log file at. If not provided, a default filepath will be used." );

		public static Option<string> OutputFile { get; } = new(
				name: Commands.Options.OutputFile,
				description: "Path to generate output file at. If not provided, a default file path will be used." );
	}
}
