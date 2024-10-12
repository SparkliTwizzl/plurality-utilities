using Petrichor.Common.Syntax;


namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Represents a data token prototype.
	/// </summary>
	public struct DataToken
	{
		/// <summary>
		/// The key which identifies this token type.
		/// </summary>
		public string Key { get; set; } = string.Empty;

		/// <summary>
		/// The maximum allowed number of instances of this token type.
		/// </summary>
		public int MaxAllowed { get; set; } = int.MaxValue;

		/// <summary>
		/// The minimum required number of instances of this token type.
		/// </summary>
		public int MinRequired { get; set; } = 0;


		/// <summary>
		/// Initializes a new instance of the <see cref="DataToken"/> struct.
		/// </summary>
		public DataToken() { }

		/// <summary>
		/// Qualifies the token's key with a token value separator sequence.
		/// </summary>
		/// <returns>Qualified key string.</returns>
		public readonly string QualifyKey() => $"{Key}{ControlSequences.TokenKeyDelimiter}";
	}
}
