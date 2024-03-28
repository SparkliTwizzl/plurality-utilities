using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class DataRegionParser<T> : IDataRegionParser<T> where T : class, new()
	{
		private const int DefaultIndentLevel = 0;
		private const int DefaultLinesParsed = 0;
		private const int DefaultTokenInstancesParsed = 0;

		private int IndentLevel { get; set; } = 0;
		private bool IsParsingFinished { get; set; } = false;
		private Func<T, T> PostParseHandler { get; set; } = ( T result ) => result;
		private Func<T> PreParseHandler { get; set; } = () => new T();
		private int RegionStartLineNumber { get; set; } = -1;
		private Dictionary<string, Func<IndexedString[], int, T, ProcessedRegionData<T>>> TokenHandlers { get; set; } = new();


		public int LinesParsed { get; private set; } = 0;
		public Dictionary<string, int> MaxAllowedTokenInstances { get; private set; } = new();
		public Dictionary<string, int> MinRequiredTokenInstances { get; private set; } = new();
		public DataToken RegionToken { get; private set; } = new();
		public Dictionary<string, int> TokenInstancesParsed { get; private set; } = new();


		public DataRegionParser() { }
		public DataRegionParser( DataRegionParserDescriptor<T> descriptor )
		{
			PostParseHandler = descriptor.PostParseHandler;
			PreParseHandler = descriptor.PreParseHandler;
			RegionToken = descriptor.RegionToken;
			StoreTokenHandlers( descriptor.TokenHandlers );
		}


		public void AddTokenHandler( DataToken token, Func<IndexedString[], int, T, ProcessedRegionData<T>> handler ) => StoreTokenHandler( token, handler );

		public void CancelParsing() => IsParsingFinished = true;

		public T Parse( IndexedString[] regionData )
		{
			var token = new StringToken( regionData[ 0 ] );
			RegionStartLineNumber = token.LineNumber;
			var regionTokenValue = token.Value != string.Empty ? $" (\"{token.Value}\")" : string.Empty;
			var taskMessage = $"Parse \"{RegionToken.Key}\" region{regionTokenValue}";
			Log.Start( taskMessage, token.LineNumber );

			TryAddDefaultControlTokenHandlers();
			PopulateTokenInstanceCountKeys();
			var result = PreParseHandler();
			result = ParseAllTokens( regionData, result );
			ValidateIndentLevel();
			ValidateTokenInstanceCounts();
			result = PostParseHandler( result );

			var regionCloseLineNumber = token.LineNumber + LinesParsed - 1;
			Log.Finish( taskMessage, regionCloseLineNumber );
			return result;
		}

		public void Reset()
		{
			IsParsingFinished = false;
			IndentLevel = DefaultIndentLevel;
			LinesParsed = DefaultLinesParsed;
			foreach ( var tokenName in TokenInstancesParsed.Keys )
			{
				TokenInstancesParsed[ tokenName ] = DefaultTokenInstancesParsed;
			}
		}


		private T ParseAllTokens( IndexedString[] regionData, T result )
		{
			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var stringToken = new StringToken( rawToken );
				var parseResult = ParseToken( stringToken, regionData, i, result );
				i += parseResult.BodySize;
				result = parseResult.Value;
				LinesParsed = i + 1;
				if ( IsParsingFinished )
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
			foreach ( var tokenName in MinRequiredTokenInstances.Keys )
			{
				_ = TokenInstancesParsed.TryAdd( tokenName, 0 );
			}
		}

		private ProcessedRegionData<T> ParseToken( StringToken stringToken, IndexedString[] regionData, int tokenStartIndex, T result )
		{
			if ( !TokenHandlers.TryGetValue( stringToken.Key, out var handler ) )
			{
				ExceptionLogger.LogAndThrow( new TokenNameException( $"An unrecognized token \"{stringToken.Key}{ControlSequences.TokenValueDivider} {stringToken.Value}\" was found in a(n) \"{RegionToken.Key}\" region." ), stringToken.LineNumber );
			}

			WarnAboutBlankTokenValues( stringToken, regionData, tokenStartIndex );

			if ( TokenInstancesParsed.TryGetValue( stringToken.Key, out var value ) )
			{
				++TokenInstancesParsed[ stringToken.Key ];
			}

			return handler!( regionData, tokenStartIndex, result );
		}

		private void StoreTokenHandler( DataToken token, Func<IndexedString[], int, T, ProcessedRegionData<T>> handler )
			=> StoreTokenHandlers( new Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedRegionData<T>>>() { { token, handler } } );

		private void StoreTokenHandlers( Dictionary<DataToken, Func<IndexedString[], int, T, ProcessedRegionData<T>>> rawHandlers )
		{
			foreach ( var handler in rawHandlers )
			{
				var token = handler.Key;
				var callback = handler.Value;

				TokenHandlers.Add( token.Key, callback );
				MaxAllowedTokenInstances.Add( token.Key, token.MaxAllowed );
				MinRequiredTokenInstances.Add( token.Key, token.MinRequired );
			}
		}

		private void TryAddDefaultControlTokenHandlers()
		{
			var regionCloseTokenHandler = ( IndexedString[] regionData, int currentLine, T result ) =>
			{
				--IndentLevel;

				if ( IndentLevel < 0 )
				{
					var lineNumber = regionData[ currentLine ].LineNumber;
					ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found in a(n) \"{RegionToken.Key}\" region." ), lineNumber );
				}

				if ( IndentLevel == 0 )
				{
					LinesParsed = currentLine + 1;
					IsParsingFinished = true;
				}

				return new ProcessedRegionData<T>( result );
			};

			var regionOpenTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, T result ) =>
			{
				++IndentLevel;
				return new ProcessedRegionData<T>( result );
			};

			_ = TokenHandlers.TryAdd( Tokens.BlankLine.Key, IDataRegionParser<T>.InertHandler );
			_ = TokenHandlers.TryAdd( Tokens.RegionClose.Key, regionCloseTokenHandler );
			_ = TokenHandlers.TryAdd( Tokens.RegionOpen.Key, regionOpenTokenHandler );
			_ = TokenHandlers.TryAdd( RegionToken.Key, IDataRegionParser<T>.InertHandler ); // this removes the need to offset the token start index in handlers to skip the region name token
		}

		private void ValidateIndentLevel()
		{
			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a(n) \"{RegionToken.Key}\" region." ), RegionStartLineNumber );
			}
		}

		private void ValidateTokenInstanceCounts()
		{
			foreach ( var tokenName in TokenInstancesParsed.Keys )
			{
				var instances = TokenInstancesParsed[ tokenName ];
				_ = MinRequiredTokenInstances.TryGetValue( tokenName, out var minRequiredInstances );
				_ = MaxAllowedTokenInstances.TryGetValue( tokenName, out var maxAllowedInstances );

				var hasTooManyInstances = instances > maxAllowedInstances;
				var hasTooFewInstances = instances < minRequiredInstances;

				var quantityWord = hasTooFewInstances ? "few" : "many";
				if ( hasTooFewInstances || hasTooManyInstances )
				{
					ExceptionLogger.LogAndThrow( new TokenCountException( $"A(n) \"{RegionToken.Key}\" region has too {quantityWord} \"{tokenName}\" tokens (Has: {instances} / Requires: Between {minRequiredInstances} and {maxAllowedInstances})." ), RegionStartLineNumber );
				}
			}
		}

		private static void WarnAboutBlankTokenValues( StringToken token, IndexedString[] regionData, int tokenStartIndex )
		{
			var isTokenNameBlank = token.Key == string.Empty;
			if ( isTokenNameBlank )
			{
				return;
			}

			var isTokenValueBlank = token.Value == string.Empty;
			if ( !isTokenValueBlank )
			{
				return;
			}

			var isTokenRegionBoundary = token.Key == Tokens.RegionOpen.Key || token.Key == Tokens.RegionClose.Key;
			if ( isTokenRegionBoundary )
			{
				return;
			}

			StringToken? nextToken = tokenStartIndex < regionData.Length - 1 ? new( regionData[ tokenStartIndex + 1 ] ) : null;
			var hasNextToken = nextToken is not null;
			var isTokenARegion = hasNextToken && nextToken!.Key == Tokens.RegionOpen.Key;
			if ( !isTokenARegion )
			{
				Log.Warning( $"A(n) \"{token.Key}\" token has no value. You can ignore this warning if it is intentional.", token.LineNumber );
			}
		}
	}
}
