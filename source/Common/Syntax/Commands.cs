namespace Petrichor.Common.Syntax
{
	/// <summary>
	/// Contains command constants and related parameter and argument definitions.
	/// </summary>
	public static class Commands
	{
		/// <summary>
		/// Indicates that no command was found.
		/// </summary>
		public const string None = "none";

		/// <summary>
		/// Indicates that an unrecognized command was found.
		/// </summary>
		public const string Some = "some";

		/// <summary>
		/// Contains the parameter constants used in commands.
		/// </summary>
		public static class Parameters
		{
			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.AutoExit"/> option.
			/// </summary>
			public const string AutoExit = "--autoExit";

			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.InputFile"/> option.
			/// </summary>
			public const string InputFile = "--inputFile";

			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.LogFile"/> option.
			/// </summary>
			public const string LogFile = "--logFile";

			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.LogMode"/> option.
			/// </summary>
			public const string LogMode = "--logMode";

			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.OutputFile"/> option.
			/// </summary>
			public const string OutputFile = "--outputFile";
		}

		/// <summary>
		/// Contains the argument constants used in commands.
		/// </summary>
		public static class Arguments
		{
			/// <summary>
			/// Use with <see cref="Parameters.LogMode"/> to enable logging to all destinations.
			/// </summary>
			public const string LogModeAll = "all";

			/// <summary>
			/// Use with <see cref="Parameters.LogMode"/> to enable logging to console.
			/// </summary>
			public const string LogModeConsoleOnly = "consoleOnly";

			/// <summary>
			/// Default value for <see cref="Parameters.LogMode"/> if not set by user.
			/// </summary>
			public static readonly string LogModeDefault = LogModeAll;

			/// <summary>
			/// Use with <see cref="Parameters.LogMode"/> to disable all logging.
			/// </summary>
			public const string LogModeNone = "none";

			/// <summary>
			/// Use with <see cref="Parameters.LogMode"/> to enable logging to file.
			/// </summary>
			public const string LogModeFileOnly = "fileOnly";
		}

		/// <summary>
		/// Maps token keys to command parameters.
		/// Allows mapping command options to command parameters.
		/// </summary>
		public static Dictionary<string, string> CommandParameterMap => new()
		{
			{ TokenPrototypes.AutoExit.Key, Parameters.AutoExit },
			{ TokenPrototypes.InputFile.Key, Parameters.InputFile },
			{ TokenPrototypes.LogFile.Key, Parameters.LogFile },
			{ TokenPrototypes.LogMode.Key, Parameters.LogMode },
			{ TokenPrototypes.OutputFile.Key, Parameters.OutputFile },
		};
	}
}
