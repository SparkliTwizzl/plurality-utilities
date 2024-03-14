﻿using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Logging;

namespace Petrichor.Common.Utilities
{
	public class RegionParser< T > : IRegionParser< T > where T : new()
	{
		private int IndentLevel { get; set; } = 0;
		private Dictionary< string, Func<string[], int, T, DataToken< T >>> TokenHandlers { get; set; }


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 0;
		public string RegionName { get; private set; }
		public int RegionsParsed { get; private set; } = 0;


		public RegionParser( string regionName, int maxRegionsAllowed, Dictionary< string, Func<string[], int, T, DataToken< T >>> tokenHandlers )
		{
			MaxRegionsAllowed = maxRegionsAllowed;
			RegionName = regionName;
			TokenHandlers = tokenHandlers;
		}

		public DataToken< T > Parse( string[] regionData )
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
				var isTokenRecognized = false;
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
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing curly brace was found when parsing a { RegionName } region" ) );
					}

					if ( IndentLevel == 0 )
					{
						LinesParsed = i + 1;
						break;
					}

					continue;
				}

				foreach ( var tokenHandler in TokenHandlers )
				{
					var recognizedTokenName = tokenHandler.Key;
					if ( token.Name == recognizedTokenName )
					{
						isTokenRecognized = true;
						var handler = tokenHandler.Value;
						var dataToken = handler( token, result );
						result = dataToken.Value;
						i += dataToken.TokenLinesLength;
						break;
					}
				}

				if ( !isTokenRecognized )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{ rawToken.Trim() }\") was found in a { RegionName } region" ) );
				}
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found in a { RegionName } region" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinish( taskMessage );
			return result;
		}
	}
}
