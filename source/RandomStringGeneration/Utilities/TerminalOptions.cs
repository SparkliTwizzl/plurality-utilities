using Petrichor.Common.Exceptions;
using Petrichor.Logging.Utilities;
using Petrichor.RandomStringGeneration.Syntax;
using System.CommandLine;


namespace Petrichor.RandomStringGeneration.Utilities
{
	/// <summary>
	/// Provides terminal options and default file paths for input, output, and logging.
	/// </summary>
	public static class TerminalOptions
	{
		/// <summary>
		/// Gets a command-line option to specify the allowed character set for random string generation.
		/// </summary>
		public static Option<string> AllowedCharacters { get; } = new(
			name: Commands.Parameters.AllowedCharacters,
			description: "Set of characters that random strings will be generated from.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Arguments.AllowedCharactersDefault;
				}

				var argument = result.Tokens[ 0 ].Value;
				if ( argument.Length < 1 )
				{
					result.ErrorMessage = $"Command option \"{Commands.Parameters.AllowedCharacters}\" argument must be a string of 1 or more characters.";
					ExceptionLogger.LogAndThrow( new CommandException( result.ErrorMessage ) );
					return string.Empty;
				}

				return argument;
			} )
			{
				Arity = ArgumentArity.ZeroOrOne,
				AllowMultipleArgumentsPerToken = false,
			};

		/// <summary>
		/// Gets a command-line option to specify the number of random strings to generate.
		/// </summary>
		public static Option<int> StringCount { get; } = new(
			name: Commands.Parameters.StringCount,
			description: "Number of random strings to generate.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Arguments.StringCountDefault;
				}

				_ = int.TryParse( result.Tokens[ 0 ].Value, out var argument);
				if ( argument < 1 )
				{
					ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Parameters.StringCount}\" argument must be a positive integer." ) );
					return 1;
				}

				return argument;
			} )
			{
				Arity = ArgumentArity.ZeroOrOne,
				AllowMultipleArgumentsPerToken = false,
			};

		/// <summary>
		/// Gets a command-line option to specify the length of each random string.
		/// </summary>
		public static Option<int> StringLength { get; } = new(
			name: Commands.Parameters.StringLength,
			description: "Length of random strings.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Arguments.StringLengthDefault;
				}

				_ = int.TryParse( result.Tokens[ 0 ].Value, out var argument);
				if ( argument < 1 )
				{
					ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Parameters.StringCount}\" argument must be a positive integer." ) );
					return 1;
				}

				return argument;
			} )
			{
				Arity = ArgumentArity.ZeroOrOne,
				AllowMultipleArgumentsPerToken = false,
			};
	}
}
