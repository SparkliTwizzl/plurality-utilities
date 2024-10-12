namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Describes a data token prototype and how it should be parsed.
	/// </summary>
	/// <typeparam name="T">
	/// The type representing the parsed data.
	/// Must be a reference type with a parameterless constructor.
	/// </typeparam>
	public struct TokenParseDescriptor<T> where T : class, new()
	{
		/// <summary>
		/// A function to be executed after the token is parsed.
		/// Used to modify the parsed result.
		/// Defaults to returning the result argument unaltered.
		/// </summary>
		public Func<T, T> PostParseAction { get; set; } = (T result) => result;

		/// <summary>
		/// A function to be executed before the token is parsed.
		/// Used to prepare the input for parsing.
		/// Defaults to initialzing a new instance of <typeparamref name="T"/>.
		/// </summary>
		public Func<T, T> PreParseAction { get; set; } = (T input) => new T();

		/// <summary>
		/// Token prototype used to identify tokens to be parsed.
		/// </summary>
		public DataToken TokenPrototype { get; set; } = new();

		/// <summary>
		/// Maps subtoken prototypes to corresponding handling functions.
		/// </summary>
		public Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedTokenData<T>>> SubTokenHandlers { get; set; } = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenParseDescriptor{T}"/> struct.
		/// </summary>
		public TokenParseDescriptor() { }
	}
}
