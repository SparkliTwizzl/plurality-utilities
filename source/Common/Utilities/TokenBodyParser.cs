using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;
using Petrichor.Logging.Utilities;

namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Parses token bodies and processes them according to specified handlers.
	/// </summary>
	/// <typeparam name="T">The type of the result object.</typeparam>
	public class TokenBodyParser<T> : ITokenBodyParser<T> where T : class, new()
	{
		private const int DefaultIndentLevel = 0;
		private const int DefaultLinesParsed = 0;
		private const int DefaultTokenInstancesParsed = 0;

		private int CurrentIndentLevel { get; set; } = 0;
		private bool IsParsingCompleted { get; set; } = false;
		private Func<T, T> PostParseAction { get; set; } = (T result) => result;
		private Func<T, T> PreParseAction { get; set; } = (T input) => new T();
		private int StartLineNumber { get; set; } = -1;
		private Dictionary<string, Func<IndexedString[], int, T, ProcessedTokenData<T>>> TokenHandlerMap { get; set; } = new();


		/// <inheritdoc />
		public int TotalLinesParsed { get; private set; } = 0;

		/// <inheritdoc />
		public Dictionary<string, int> MaxTokenInstances { get; private set; } = new();

		/// <inheritdoc />
		public Dictionary<string, int> MinTokenInstances { get; private set; } = new();

		/// <inheritdoc />
		public DataToken TokenPrototype { get; private set; } = new();

		/// <inheritdoc />
		public Dictionary<string, int> ParsedTokenInstances { get; private set; } = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenBodyParser{T}"/> class.
		/// </summary>
		public TokenBodyParser() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenBodyParser{T}"/> class with a descriptor.
		/// </summary>
		/// <param name="descriptor">The token parse descriptor with initialization data.</param>
		public TokenBodyParser(TokenParseDescriptor<T> descriptor)
		{
			PostParseAction = descriptor.PostParseAction;
			PreParseAction = descriptor.PreParseAction;
			TokenPrototype = descriptor.TokenPrototype;
			RegisterTokenHandlers(descriptor.SubTokenHandlers);
		}

		/// <inheritdoc />
		public void AddTokenHandler(DataToken tokenPrototype, Func<IndexedString[], int, T, ProcessedTokenData<T>> handler) => RegisterTokenHandler(tokenPrototype, handler);

		/// <inheritdoc />
		public void CancelParsing() => IsParsingCompleted = true;

		/// <inheritdoc />
		public T Parse(IndexedString[] data, T? input = null)
		{
			if (data.Length < 1)
			{
				ExceptionLogger.LogAndThrow(new TokenValueException("Attempted to parse a nonexistent token body."));
			}

			var token = new StringToken(data[0]);
			StartLineNumber = token.LineNumber;
			var tokenValueString = token.TokenValue != string.Empty ? $" (\"{token.TokenValue}\")" : string.Empty;
			var taskMessage = $"Parse \"{TokenPrototype.Key}\" region{tokenValueString}";
			Logger.Start(taskMessage, token.LineNumber);

			AddDefaultControlTokenHandlers();
			PopulateTokenInstanceCountKeys();
			var result = PreParseAction(input ?? new T());
			result = ParseAllTokens(data, result);
			CheckIndentLevel();
			CheckTokenInstanceCounts();
			result = PostParseAction(result);

			var regionCloseLineNumber = token.LineNumber + TotalLinesParsed - 1;
			Logger.Finish(taskMessage, regionCloseLineNumber);
			return result;
		}

		/// <inheritdoc />
		public void Reset()
		{
			IsParsingCompleted = false;
			CurrentIndentLevel = DefaultIndentLevel;
			TotalLinesParsed = DefaultLinesParsed;
			foreach (var tokenName in ParsedTokenInstances.Keys)
			{
				ParsedTokenInstances[tokenName] = DefaultTokenInstancesParsed;
			}
		}


		private T ParseAllTokens(IndexedString[] bodyData, T result)
		{
			for (var i = 0; i < bodyData.Length; ++i)
			{
				var rawToken = bodyData[i];
				var stringToken = new StringToken(rawToken);
				var parseResult = ParseToken(stringToken, bodyData, i, result);
				i += parseResult.BodySize;
				result = parseResult.Value;
				TotalLinesParsed = i + 1;
				if (IsParsingCompleted)
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Ensures all token instances are counted, even if no instances are parsed.
		/// </summary>
		private void PopulateTokenInstanceCountKeys()
		{
			foreach (var tokenName in MinTokenInstances.Keys)
			{
				_ = ParsedTokenInstances.TryAdd(tokenName, 0);
			}
		}

		private ProcessedTokenData<T> ParseToken(StringToken stringToken, IndexedString[] bodyData, int tokenStartIndex, T result)
		{
			if (!TokenHandlerMap.TryGetValue(stringToken.TokenKey, out var handler))
			{
				ExceptionLogger.LogAndThrow(new TokenNameException($"An unrecognized token \"{stringToken.TokenKey}{ControlSequences.TokenKeyDelimiter} {stringToken.TokenValue}\" was found in a(n) \"{TokenPrototype.Key}\" region."), stringToken.LineNumber);
			}

			LogBlankTokenWarnings(stringToken, bodyData, tokenStartIndex);

			if (ParsedTokenInstances.TryGetValue(stringToken.TokenKey, out var value))
			{
				++ParsedTokenInstances[stringToken.TokenKey];
			}

			return handler!(bodyData, tokenStartIndex, result);
		}

		private void RegisterTokenHandler(DataToken token, Func<IndexedString[], int, T, ProcessedTokenData<T>> handler)
			=> RegisterTokenHandlers(new Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedTokenData<T>>>() { { token, handler } });

		private void RegisterTokenHandlers(Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedTokenData<T>>> rawHandlers)
		{
			foreach (var handler in rawHandlers)
			{
				var token = handler.Key;
				var callback = handler.Value;

				TokenHandlerMap.Add(token.Key, callback);
				MaxTokenInstances.Add(token.Key, token.MaxAllowed);
				MinTokenInstances.Add(token.Key, token.MinRequired);
			}
		}

		private void AddDefaultControlTokenHandlers()
		{
			var regionCloseTokenHandler = (IndexedString[] bodyData, int currentLine, T result) =>
			{
				--CurrentIndentLevel;

				if (CurrentIndentLevel < 0)
				{
					var lineNumber = bodyData[currentLine].LineNumber;
					ExceptionLogger.LogAndThrow(new BracketException($"A mismatched closing bracket was found in a(n) \"{TokenPrototype.Key}\" region."), lineNumber);
				}

				if (CurrentIndentLevel == 0)
				{
					TotalLinesParsed = currentLine + 1;
					IsParsingCompleted = true;
				}

				return new ProcessedTokenData<T>(result);
			};

			var regionOpenTokenHandler = (IndexedString[] bodyData, int tokenStartIndex, T result) =>
			{
				++CurrentIndentLevel;
				return new ProcessedTokenData<T>(result);
			};

			_ = TokenHandlerMap.TryAdd(TokenPrototypes.BlankLine.Key, ITokenBodyParser<T>.InertHandler);
			_ = TokenHandlerMap.TryAdd(TokenPrototypes.TokenBodyClose.Key, regionCloseTokenHandler);
			_ = TokenHandlerMap.TryAdd(TokenPrototypes.TokenBodyOpen.Key, regionOpenTokenHandler);
			_ = TokenHandlerMap.TryAdd(TokenPrototype.Key, ITokenBodyParser<T>.InertHandler);
			// setting the prototype's key to an inert handler removes the need to offset the start index
			//   in handlers in order to skip the valueless token that just starts a data container
		}

		private void CheckIndentLevel()
		{
			if (CurrentIndentLevel != 0)
			{
				ExceptionLogger.LogAndThrow(new BracketException($"A mismatched curly brace was found in a(n) \"{TokenPrototype.Key}\" region."), StartLineNumber);
			}
		}

		private void CheckTokenInstanceCounts()
		{
			foreach (var tokenName in ParsedTokenInstances.Keys)
			{
				var instances = ParsedTokenInstances[tokenName];
				_ = MinTokenInstances.TryGetValue(tokenName, out var minRequiredInstances);
				_ = MaxTokenInstances.TryGetValue(tokenName, out var maxAllowedInstances);

				var hasTooManyInstances = instances > maxAllowedInstances;
				var hasTooFewInstances = instances < minRequiredInstances;

				var quantityWord = hasTooFewInstances ? "few" : "many";
				if (hasTooFewInstances || hasTooManyInstances)
				{
					ExceptionLogger.LogAndThrow(new TokenCountException($"A(n) \"{TokenPrototype.Key}\" region has too {quantityWord} \"{tokenName}\" tokens (Has: {instances} / Requires: Between {minRequiredInstances} and {maxAllowedInstances})."), StartLineNumber);
				}
			}
		}

		private static void LogBlankTokenWarnings(StringToken token, IndexedString[] bodyData, int tokenStartIndex)
		{
			var isTokenNameBlank = token.TokenKey == string.Empty;
			if (isTokenNameBlank)
			{
				return;
			}

			var isTokenValueBlank = token.TokenValue == string.Empty;
			if (!isTokenValueBlank)
			{
				return;
			}

			var isTokenRegionBoundary = token.TokenKey == TokenPrototypes.TokenBodyOpen.Key || token.TokenKey == TokenPrototypes.TokenBodyClose.Key;
			if (isTokenRegionBoundary)
			{
				return;
			}

			StringToken? nextToken = tokenStartIndex < bodyData.Length - 1 ? new(bodyData[tokenStartIndex + 1]) : null;
			var hasNextToken = nextToken is not null;
			var isTokenARegion = hasNextToken && nextToken!.TokenKey == TokenPrototypes.TokenBodyOpen.Key;
			if (!isTokenARegion)
			{
				Logger.Warning($"A(n) \"{token.TokenKey}\" token has no value. You can ignore this warning if it is intentional.", token.LineNumber);
			}
		}
	}
}
