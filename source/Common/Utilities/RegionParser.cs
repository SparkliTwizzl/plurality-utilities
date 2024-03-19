using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class RegionParser<T> : IRegionParser<T> where T : new()
	{
		private const int DefaultIndentLevel = 0;
		private const int DefaultLinesParsed = 0;
		private const int DefaultTokenInstancesParsed = 0;

		private int IndentLevel { get; set; } = 0;
		private Func<T, T> PostParseHandler { get; set; } = ( T result ) => result;
		private Func<T> PreParseHandler { get; set; } = () => new T();
		private Dictionary<string, Func<string[], int, T, RegionData<T>>> TokenHandlers { get; set; } = new();
		private bool IsParsingFinished { get; set; } = false;


		private Func<string[], int, T, RegionData<T>> RegionCloseTokenHandler => ( string[] regionData, int currentLine, T result ) =>
		{
			--IndentLevel;

			if ( IndentLevel < 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found in a(n) \"{RegionName}\" region." ) );
			}

			if ( IndentLevel == 0 )
			{
				LinesParsed = currentLine + 1;
				IsParsingFinished = true;
			}

			return new()
			{
				Value = result,
			};
		};

		/// <summary>
		/// Default logic, removes the need to offset the token start index in handlers to skip the region name token.
		/// </summary>
		private Func<string[], int, T, RegionData<T>> RegionNameTokenHandler => ( string[] regionData, int tokenStartIndex, T result ) =>
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			if ( token.Value != string.Empty )
			{
				Log.Info( $"\"{token.Name}\" value: \"{token.Value}\"" );
			}
			return new()
			{
				Value = result,
			};
		};

		private Func<string[], int, T, RegionData<T>> RegionOpenTokenHandler => ( string[] regionData, int tokenStartIndex, T result ) =>
		{
			++IndentLevel;
			return new()
			{
				Value = result,
			};
		};


		public int LinesParsed { get; private set; } = 0;
		public Dictionary<string, int> MaxAllowedTokenInstances { get; private set; } = new();
		public Dictionary<string, int> MinRequiredTokenInstances { get; private set; } = new();
		public string RegionName { get; private set; } = string.Empty;
		public Dictionary<string, int> TokenInstancesParsed { get; private set; } = new();


		public RegionParser() { }
		public RegionParser( RegionParserDescriptor<T> descriptor )
		{
			MaxAllowedTokenInstances = descriptor.MaxAllowedTokenInstances;
			MinRequiredTokenInstances = descriptor.MinRequiredTokenInstances;
			PostParseHandler = descriptor.PostParseHandler;
			PreParseHandler = descriptor.PreParseHandler;
			RegionName = descriptor.RegionName;
			TokenHandlers = descriptor.TokenHandlers;
		}


		public T Parse( string[] regionData )
		{
			var taskMessage = $"Parse \"{RegionName}\" region";
			Log.TaskStart( taskMessage );

			TryAddDefaultControlTokenHandlers();
			PopulateTokenInstanceCountKeys();
			var result = PreParseHandler();
			result = ParseAllTokens( regionData, result );
			ValidateIndentLevel();
			ValidateTokenInstanceCounts();
			result = PostParseHandler( result );

			Log.TaskFinish( taskMessage );
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


		private T ParseAllTokens( string[] regionData, T result )
		{
			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );
				var parseResult = ParseToken( token, regionData, i, result );
				i += parseResult.BodySize;
				result = parseResult.Value;

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

		private RegionData<T> ParseToken( StringToken token, string[] regionData, int tokenStartIndex, T result )
		{
			if ( !TokenHandlers.TryGetValue( token.Name, out var handler ) )
			{
				ExceptionLogger.LogAndThrow( new TokenNameException( $"An unrecognized token \"{token.Name}{OperatorChars.TokenValueDivider} {token.Value}\" was found in a(n) \"{RegionName}\" region." ) );
			}

			WarnAboutBlankTokenValues( token, regionData, tokenStartIndex );

			if ( TokenInstancesParsed.TryGetValue( token.Name, out var value ) )
			{
				++TokenInstancesParsed[ token.Name ];
			}

			return handler!( regionData, tokenStartIndex, result );
		}

		private void TryAddDefaultControlTokenHandlers()
		{
			_ = TokenHandlers.TryAdd( string.Empty, IRegionParser<T>.InertHandler );
			_ = TokenHandlers.TryAdd( Tokens.RegionClose, RegionCloseTokenHandler );
			_ = TokenHandlers.TryAdd( Tokens.RegionOpen, RegionOpenTokenHandler );
			_ = TokenHandlers.TryAdd( RegionName, RegionNameTokenHandler );
		}

		private void ValidateIndentLevel()
		{
			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a(n) \"{RegionName}\" region." ) );
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
				if ( hasTooFewInstances || hasTooManyInstances )
				{
					ExceptionLogger.LogAndThrow( new TokenCountException( $"\"{RegionName}\" regions must contain at least {minRequiredInstances} and no more than {maxAllowedInstances} \"{tokenName}\" tokens." ) );
				}
			}
		}

		private static void WarnAboutBlankTokenValues( StringToken token, string[] regionData, int tokenStartIndex )
		{
			var isTokenNameBlank = token.Name == string.Empty;
			if ( isTokenNameBlank )
			{
				return;
			}

			var isTokenValueBlank = token.Value == string.Empty;
			if ( !isTokenValueBlank )
			{
				return;
			}

			var isTokenRegionBoundary = token.Name == TokenNames.RegionOpen || token.Name == TokenNames.RegionClose;
			if ( isTokenRegionBoundary )
			{
				return;
			}

			StringToken? nextToken = tokenStartIndex < regionData.Length - 1 ? new( regionData[ tokenStartIndex + 1 ] ) : null;
			var hasNextToken = nextToken is not null;
			var isTokenARegion = hasNextToken && nextToken!.Name == TokenNames.RegionOpen;
			if ( !isTokenARegion )
			{
				Log.Warning( $"A(n) \"{token.Name}\" token has no value. You can ignore this warning if it is intentional." );
			}
		}
	}
}
