using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;
using Petrichor.RandomStringGeneration.Syntax;
using System.CommandLine;


namespace Petrichor.RandomStringGeneration.Utilities
{
	public readonly struct TerminalOptions
	{
		public static Option<string> AllowedCharacters { get; } = new(
			name: Commands.Options.AllowedCharacters,
			description: "Set of characters that random strings will be generated from.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Options.Defaults.AllowedCharactersValue;
				}

				var argument = result.Tokens[ 0 ].Value;
				if ( argument.Length < 1 )
				{
					result.ErrorMessage = $"Command option \"{Commands.Options.AllowedCharacters}\" argument must be a string of 1 or more characters.";
					ExceptionLogger.LogAndThrow( new CommandException( result.ErrorMessage ) );
					return string.Empty;
				}

				return argument;
			} )
			{
				Arity = ArgumentArity.ZeroOrOne,
				AllowMultipleArgumentsPerToken = false,
			};

		public static Option<int> StringCount { get; } = new(
			name: Commands.Options.StringCount,
			description: "Number of random strings to generate.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Options.Defaults.StringCountValue;
				}

				_ = int.TryParse( result.Tokens[ 0 ].Value, out var argument);
				if ( argument < 1 )
				{
					ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringCount}\" argument must be a positive integer." ) );
					return 1;
				}

				return argument;
			} )
			{
				Arity = ArgumentArity.ZeroOrOne,
				AllowMultipleArgumentsPerToken = false,
			};

		public static Option<int> StringLength { get; } = new(
			name: Commands.Options.StringLength,
			description: "Length of random strings.",
			isDefault: true,
			parseArgument: result =>
			{
				if ( !result.Tokens.Any() )
				{
					return Commands.Options.Defaults.StringLengthValue;
				}

				_ = int.TryParse( result.Tokens[ 0 ].Value, out var argument);
				if ( argument < 1 )
				{
					ExceptionLogger.LogAndThrow( new CommandException( $"Command option \"{Commands.Options.StringCount}\" argument must be a positive integer." ) );
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
