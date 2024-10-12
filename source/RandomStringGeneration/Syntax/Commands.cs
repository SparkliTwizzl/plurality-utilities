namespace Petrichor.RandomStringGeneration.Syntax
{
	/// <summary>
	/// Contains command constants and related parameter and argument definitions.
	/// </summary>
	public static class Commands
	{
		/// <summary>
		/// Command to trigger this module.
		/// </summary>
		public const string ModuleCommand = "generateRandomStrings";

		/// <summary>
		/// Contains the parameter constants used in commands.
		/// </summary>
		public static class Parameters
		{
			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.AllowedCharacters"/> option.
			/// </summary>
			public const string AllowedCharacters = "--allowedCharacters";

			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.StringCount"/> option.
			public const string StringCount = "--stringCount";
			
			/// <summary>
			/// Parameter for the <see cref="Utilities.TerminalOptions.StringLength"/> option.
			public const string StringLength = "--stringLength";
		}

		/// <summary>
		/// Contains the argument constants used in commands.
		/// </summary>
		public static class Arguments
		{
			/// <summary>
			/// Default value for <see cref="Parameters.AllowedCharacters"/> if not set by user.
			/// </summary>
			public const string AllowedCharactersDefault = "abcdefghijklmnopqrstuvwxyz";

			/// <summary>
			/// Default value for <see cref="Parameters.StringCount"/> if not set by user.
			/// </summary>
			public const int StringCountDefault = 1;

			/// <summary>
			/// Default value for <see cref="Parameters.StringLength"/> if not set by user.
			public const int StringLengthDefault = 10;
		}

		/// <summary>
		/// Maps token keys to command parameters.
		/// Allows mapping command options to command parameters.
		/// </summary>
		public static Dictionary<string, string> CommandParameterMap => new()
		{
			{ TokenPrototypes.AllowedCharacters.Key, Parameters.AllowedCharacters },
			{ TokenPrototypes.StringCount.Key, Parameters.StringCount },
			{ TokenPrototypes.StringLength.Key, Parameters.StringLength },
		};

	}
}
