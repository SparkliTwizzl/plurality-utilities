using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class RegionParser< T > : IRegionParser< T > where T : new()
	{
		private const int DefaultIndentLevel = 0;
		private const int DefaultLinesParsed = 0;
		private const int DefaultTokenInstancesParsed = 0;

		private int IndentLevel { get; set; } = 0;
		private Func< T, T > PostParseHandler { get; set; } = ( T result ) => result;
		private Func< T > PreParseHandler { get; set; } = () => new T();
		private Dictionary< string, Func< string[], int, T, RegionData< T > > > TokenHandlers { get; set; } = new();


		public int LinesParsed { get; private set; } = 0;
		public Dictionary< string, int > MaxAllowedTokenInstances { get; private set; } = new();
		public Dictionary< string, int > MinRequiredTokenInstances { get; private set; } = new();
		public string RegionName { get; private set; } = string.Empty;
		public Dictionary< string, int > TokenInstancesParsed { get; private set; } = new();


		public RegionParser() { }

		public RegionParser( RegionParserDescriptor< T > descriptor )
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
			var taskMessage = $"Parse \"{ RegionName }\" region";
			Log.TaskStart( taskMessage );

			// handle this region's token name with no effect; this removes the need to offset the token start index in handlers to prevent this region's token from being parsed
			_ = TokenHandlers.TryAdd( RegionName, ( string[] regionData, int tokenStartIndex, T result ) => new(){ Value = result } );
			_ = MaxAllowedTokenInstances.TryAdd( RegionName, int.MaxValue );
			_ = MinRequiredTokenInstances.TryAdd( RegionName, 0 );
			foreach ( var tokenName in MinRequiredTokenInstances.Keys )
			{
				_ = TokenInstancesParsed.TryAdd( tokenName, 0 );
			}

			var result = PreParseHandler();

			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == Tokens.RegionOpen )
				{
					++IndentLevel;
					continue;
				}

				else if ( token.Name == Tokens.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found in a(n) \"{ RegionName }\" region." ) );
					}

					if ( IndentLevel == 0 )
					{
						LinesParsed = i;
						break;
					}

					continue;
				}

				else if ( TokenHandlers.TryGetValue( token.Name, out var handler ) )
				{
					if ( token.Value == string.Empty )
					{
						StringToken? nextToken = i < regionData.Length - 1 ? new( regionData[ i + 1 ] ) : null;
						if ( nextToken is not null && nextToken.Name != TokenNames.RegionOpen )
						{
							Log.Warning( $"A(n) \"{ token.Name }\" token has no value." );
						}
					}

					++TokenInstancesParsed[ token.Name ];
					var handlerResult = handler( regionData, i, result );
					i += handlerResult.BodySize;
					result = handlerResult.Value;
					continue;
				}

				// this can only be reached if a token is not recognized and therefore not handled
				ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token \"{ token.Name }{ OperatorChars.TokenValueDivider } { token.Value }\" was found in a(n) \"{ RegionName }\" region." ) );
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a(n) \"{ RegionName }\" region." ) );
			}

			foreach ( var tokenName in TokenInstancesParsed.Keys )
			{
				var instances = TokenInstancesParsed[ tokenName ];
				_ = MinRequiredTokenInstances.TryGetValue( tokenName, out var minRequiredInstances );
				_ = MaxAllowedTokenInstances.TryGetValue( tokenName, out var maxAllowedInstances );

				var hasTooManyInstances = instances > maxAllowedInstances;
				var hasTooFewInstances = instances < minRequiredInstances;
				if ( hasTooFewInstances || hasTooManyInstances )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"\"{ RegionName }\" regions must contain at least { minRequiredInstances } and no more than { maxAllowedInstances } \"{ tokenName }\" tokens." ) );
				}
			}

			result = PostParseHandler( result );

			Log.TaskFinish( taskMessage );
			return result;
		}

		public void Reset()
		{
			IndentLevel = DefaultIndentLevel;
			LinesParsed = DefaultLinesParsed;
			foreach ( var tokenName in TokenInstancesParsed.Keys )
			{
				TokenInstancesParsed[ tokenName ] = DefaultTokenInstancesParsed;
			}
		}
	}
}
