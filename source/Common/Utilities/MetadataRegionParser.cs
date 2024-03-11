using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Syntax;
using Petrichor.Logging;
using System.Data;


namespace Petrichor.Common.Utilities
{
	public class MetadataRegionParser : IMetadataRegionParser
	{
		private bool HasParsedMinimumVersionToken { get; set; } = false;
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => TokenNames.MetadataRegion;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed => 1;
		public static string RegionIsValidMessage => $"Region data is valid for this version of { AppInfo.AppName }";
		public int RegionsParsed { get; private set; } = 0;


		public string Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: { RegionName }";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than { MaxRegionsAllowed } { RegionName } regions" ) );
			}

			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );
				var isParsingFinished = false;

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == TokenNames.RegionOpen )
				{
					++IndentLevel;
				}

				else if ( token.Name == TokenNames.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched close bracket was found when parsing region: {RegionName}" ) );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == TokenNames.MinimumVersion )
				{
					if ( HasParsedMinimumVersionToken )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"{ RegionName } region cannot contain more than 1 { TokenNames.MinimumVersion } token" ) );
					}
					RejectUnsupportedVersions( token.Value );
					HasParsedMinimumVersionToken = true;
				}

				else
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{ rawToken.Trim() }\") was found when parsing region: { RegionName }" ) );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i + 1;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched open bracket was found when parsing region: { RegionName }" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinish( taskMessage );
			return RegionIsValidMessage;
		}


		private static void RejectUnsupportedVersions( string version )
		{
			if ( string.IsNullOrEmpty( version ) )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"{ TokenNames.MinimumVersion } token cannot be blank" ) );
			}

			if ( !AppVersion.IsVersionSupported( version ) )
			{
				ExceptionLogger.LogAndThrow( new VersionNotFoundException( $"Input file version ({ version }) is not supported by this version of { AppInfo.AppName }" ) );
			}
		}
	}
}
