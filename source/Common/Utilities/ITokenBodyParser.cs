using Petrichor.Common.Containers;


namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Defines a parser for processing token bodies from indexed string data.
	/// </summary>
	/// <typeparam name="T">The type of result produced by the parser.</typeparam>
	public interface ITokenBodyParser<T> where T : class, new()
	{
		/// <summary>
		/// A no-op token handler function that returns the provided result without modification.
		/// </summary>
		static Func<IndexedString[], int, T, ProcessedTokenData<T>> InertHandler => (IndexedString[] data, int tokenStartIndex, T result) => new() { Value = result };

		/// <summary>
		/// Gets the total number of lines that have been parsed.
		/// </summary>
		int TotalLinesParsed { get; }

		/// <summary>
		/// Gets the token prototype used to identify tokens to be parsed.
		/// </summary>
		DataToken TokenPrototype { get; }

		/// <summary>
		/// Gets the number of instances of each parsed token type.
		/// </summary>
		Dictionary<string, int> ParsedTokenInstances { get; }

		/// <summary>
		/// Adds a handler for processing a specific token during parsing.
		/// </summary>
		/// <param name="tokenPrototype">The token prototype to associate with the handler.</param>
		/// <param name="handler">The handler function that processes the token data.</param>
		void AddTokenHandler(DataToken tokenPrototype, Func<IndexedString[], int, T, ProcessedTokenData<T>> handler);

		/// <summary>
		/// Cancels the parsing process.
		/// </summary>
		void CancelParsing();

		/// <summary>
		/// Parses the provided indexed string data.
		/// </summary>
		/// <param name="data">The data to be parsed.</param>
		/// <param name="input">An optional input of type <typeparamref name="T"/> to start with.</param>
		/// <returns>An object of type <typeparamref name="T"/> representing the result of the parsing operation.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
		/// <exception cref="Exceptions.TokenNameException">Thrown when an unrecognized token is encountered.</exception>
		/// <exception cref="Exceptions.TokenValueException">Thrown when attempting to parse a nonexistent token body, an invalid token value is parsed, indent level is nonzero after parsing, or the number of parsed token instances does not match the expected counts.</exception>
		T Parse(IndexedString[] data, T? input = null);

		/// <summary>
		/// Resets the parser state.
		/// </summary>
		void Reset();
	}
}
