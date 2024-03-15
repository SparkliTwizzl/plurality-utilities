using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class RegionParser< T > : IRegionParser< T > where T : class, new()
	{
		private int IndentLevel { get; set; } = 0;
		private Dictionary< string, Func< string[], int, T, RegionData< T > > > TokenHandlers { get; set; }


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public bool HasParsedMinRequiredRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 0;
		public int MinRegionsRequired { get; private set; } = 0;
		public string RegionName { get; private set; }
		public int RegionsParsed { get; private set; } = 0;


		public RegionParser( RegionParserDescriptor< T > descriptor )
		{
			MaxRegionsAllowed = descriptor.MaxRegionsAllowed;
			MinRegionsRequired = descriptor.MinRegionsRequired;
			RegionName = descriptor.RegionName;
			TokenHandlers = descriptor.TokenHandlers;
		}

		public RegionParser( string regionName, int maxRegionsAllowed, int minRegionsRequired, Dictionary< string, Func< string[], int, T, RegionData< T > > > tokenHandlers )
		{
			MaxRegionsAllowed = maxRegionsAllowed;
			MinRegionsRequired = minRegionsRequired;
			RegionName = regionName;
			TokenHandlers = tokenHandlers;
		}

		public T Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: { RegionName }";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than { MaxRegionsAllowed } { RegionName } regions" ) );
			}

			var result = new T();

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
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found when parsing region: { RegionName }" ) );
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
					var handlerResult = handler( regionData, i, result );
					i += handlerResult.BodySize;
					result = handlerResult.Value;
					continue;
				}

				// this can only be reached if a token is not recognized and therefore not handled
				ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{ rawToken.Trim() }\") was found in a { RegionName } region" ) );
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a { RegionName } region" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;
			HasParsedMinRequiredRegions = RegionsParsed >= MinRegionsRequired;

			if ( !HasParsedMinRequiredRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file must contain at least { MinRegionsRequired } { RegionName } regions" ) );
			}

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
