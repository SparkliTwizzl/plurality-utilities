using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Provides methods to handle tokens for shortcut script entries.
	/// </summary>
	public static class EntryHandler
	{
		/// <summary>
		/// Handles the color token and updates the entry with the token value.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the token value.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> ColorTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.Color = token.TokenValue;
			return new ProcessedTokenData<Entry>(result);
		}

		/// <summary>
		/// Handles the decoration token and updates the entry with the token value.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the token value.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> DecorationTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.Decoration = token.TokenValue;
			return new ProcessedTokenData<Entry>(result);
		}

		/// <summary>
		/// Handles the ID token and updates the entry with the token value.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the token value.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> IDTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.ID = token.TokenValue;
			return new ProcessedTokenData<Entry>(result);
		}

		/// <summary>
		/// Handles the name token and updates the entry with the parsed name.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the parsed name.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> NameTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.Names.Add(ParseNameFromToken(token));
			return new ProcessedTokenData<Entry>(result);
		}

		/// <summary>
		/// Handles the last name token and updates the entry with the parsed last name.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the parsed last name.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> LastNameTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.LastName = ParseNameFromToken(token);
			return new ProcessedTokenData<Entry>(result);
		}

		/// <summary>
		/// Handles the pronoun token and updates the entry with the token value.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token in the body data.</param>
		/// <param name="result">The entry to update with the token value.</param>
		/// <returns>A <see cref="ProcessedTokenData{Entry}"/> object containing the updated entry.</returns>
		public static ProcessedTokenData<Entry> PronounTokenHandler(IndexedString[] bodyData, int tokenStartIndex, Entry result)
		{
			var token = new StringToken(bodyData[tokenStartIndex]);
			result.Pronoun = token.TokenValue;
			return new ProcessedTokenData<Entry>(result);
		}


		/// <summary>
		/// Parses a name from the given token.
		/// </summary>
		/// <param name="token">The token containing the name value.</param>
		/// <returns>A <see cref="Name"/> object parsed from the token value.</returns>
		/// <exception cref="TokenValueException">Thrown when the token value is invalid.</exception>
		private static Name ParseNameFromToken(StringToken token)
		{
			var components = token.TokenValue.Split('@');
			if (components.Length != 2)
			{
				ExceptionLogger.LogAndThrow(new TokenValueException($"A {token.TokenKey} token had an invalid value ( \"{token.TokenValue}\" )."), token.LineNumber);
			}

			var value = components[0].Trim();
			var tag = components[1].Trim();
			var name = new Name(value, tag);
			return name;
		}
	}
}
