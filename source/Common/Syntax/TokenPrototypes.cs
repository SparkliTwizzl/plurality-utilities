using Petrichor.Common.Containers;


namespace Petrichor.Common.Syntax
{
	/// <summary>
	/// Represents a collection of data token prototypes that are common to all modules.
	/// </summary>
	public static class TokenPrototypes
	{
		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.AutoExit"/> option.
		/// </summary>
		public static DataToken AutoExit => new()
		{
			Key = "auto-exit",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing a blank line.
		/// </summary>
		public static DataToken BlankLine => new()
		{
			Key = string.Empty,
		};

		/// <summary>
		/// Gets the token representing a terminal command.
		/// </summary>
		public static DataToken Command => new()
		{
			Key = "command",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.InputFile"/> options.
		/// </summary>
		public static DataToken InputFile => new()
		{
			Key = "input-file",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing a line comment.
		/// </summary>
		public static DataToken LineComment => new()
		{
			Key = ControlSequences.LineComment,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.LogFile"/> option.
		/// </summary>
		public static DataToken LogFile => new()
		{
			Key = "log-file",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.LogMode"/> option.
		/// </summary>
		public static DataToken LogMode => new()
		{
			Key = "log-mode",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing input file metadata.
		/// </summary>
		public static DataToken Metadata => new()
		{
			Key = "metadata",
			MaxAllowed = 1,
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing minimum required version information.
		/// </summary>
		public static DataToken MinimumVersion => new()
		{
			Key = "minimum-version",
			MaxAllowed = 1,
			MinRequired = 1,
		};

		/// <summary>
		/// Gets the token representing the <see cref="Utilities.TerminalOptions.OutputFile"/> option.
		/// </summary>
		public static DataToken OutputFile => new()
		{
			Key = "output-file",
			MaxAllowed = 1,
		};

		/// <summary>
		/// Gets the token representing the end of a token body.
		/// </summary>
		public static DataToken TokenBodyClose => new()
		{
			Key = ControlSequences.TokenBodyClose.ToString(),
		};

		/// <summary>
		/// Gets the token representing the start of a token body.
		/// </summary>
		public static DataToken TokenBodyOpen => new()
		{
			Key = ControlSequences.TokenBodyOpen.ToString(),
		};


		/// <summary>
		/// Maps token keys to token prototypes.
		/// Allows mapping command options to token prototypes.
		/// </summary>
		public static Dictionary<string, DataToken> CommandTokenMap => new()
		{
			{ AutoExit.Key, AutoExit },
			{ InputFile.Key, InputFile },
			{ LogFile.Key, LogFile },
			{ LogMode.Key, LogMode },
			{ OutputFile.Key, OutputFile },
		};
	}
}
