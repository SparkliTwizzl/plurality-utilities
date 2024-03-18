using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class RegionParser< T > : IRegionParser< T > where T : class, new()
	{
		private int IndentLevel { get; set; } = 0;
		private Func< T, T > PostParseHandler { get; set; } = ( T result ) => new T();
		private Func< T > PreParseHandler { get; set; } = () => new T();
		private Dictionary< string, Func< string[], int, T, RegionData< T > > > TokenHandlers { get; set; } = new();


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public bool HasParsedMinRequiredRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public Dictionary< string, int > MaxAllowedTokenInstances { get; private set; } = new();
		public int MaxRegionsAllowed { get; private set; } = 0;
		public int MinRegionsRequired { get; private set; } = 0;
		public Dictionary< string, int > MinRequiredTokenInstances { get; private set; } = new();
		public string RegionName { get; private set; } = string.Empty;
		public int RegionsParsed { get; private set; } = 0;
		public Dictionary< string, int > TokenInstancesParsed { get; private set; } = new();


		public RegionParser() { }

		public RegionParser( RegionParserDescriptor< T > descriptor )
		{
			MaxAllowedTokenInstances = descriptor.MaxAllowedTokenInstances;
			MaxRegionsAllowed = descriptor.MaxRegionsAllowed;
			MinRegionsRequired = descriptor.MinRegionsRequired;
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

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than { MaxRegionsAllowed } \"{ RegionName }\" regions." ) );
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

				else if ( token.Name == Syntax.Tokens.RegionOpen )
				{
					++IndentLevel;
					continue;
				}

				else if ( token.Name == Syntax.Tokens.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found in a \"{ RegionName }\" region." ) );
					}

					if ( IndentLevel == 0 )
					{
						LinesParsed = i + 1;
						break;
					}

					continue;
				}

				if ( TokenHandlers.TryGetValue( token.Name, out var handler ) )
				{
					if ( !TokenInstancesParsed.ContainsKey( token.Name ) )
					{
						TokenInstancesParsed.Add( token.Name, 0 );
					}

					++TokenInstancesParsed[ token.Name ];
					var handlerResult = handler( regionData, i, result );
					i += handlerResult.BodySize;
					result = handlerResult.Value;
					continue;
				}

				// this can only be reached if a token is not recognized and therefore not handled
				ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{ rawToken.Trim() }\") was found in a \"{ RegionName }\" region." ) );
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a \"{ RegionName }\" region." ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;
			HasParsedMinRequiredRegions = RegionsParsed >= MinRegionsRequired;

			foreach ( var tokenName in TokenInstancesParsed.Keys )
			{
				var instances = TokenInstancesParsed[ tokenName ];
				var minRequiredInstances = MinRequiredTokenInstances[ tokenName ];
				var maxAllowedInstances = MaxAllowedTokenInstances[ tokenName ];

				var hasTooManyInstances = instances > maxAllowedInstances;
				var hasTooFewInstances = instances < minRequiredInstances;

				if ( hasTooFewInstances || hasTooManyInstances )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"\"{ RegionName }\" regions must contain at least { minRequiredInstances } and no more than { maxAllowedInstances } \"{ tokenName }\" tokens." ) );
				}
			}

			if ( !HasParsedMinRequiredRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file must contain at least { MinRegionsRequired } \"{ RegionName }\" regions." ) );
			}

			result = PostParseHandler( result );

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
