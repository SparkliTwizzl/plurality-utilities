using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Logging;
using System.Data;


namespace Petrichor.Common.Utilities
{
	public class MetadataRegionParser : IMetadataRegionParser
	{
		private bool HasParsedMinimumVersionToken { get; set; } = false;
		private int IndentLevel { get; set; } = 0;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public static string RegionIsValidMessage => $"Region data is valid for this version of {AppInfo.AppName}";
		public int RegionsParsed { get; private set; } = 0;


		public string Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {CommonSyntax.MetadataRegionTokenName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {CommonSyntax.MetadataRegionTokenName} regions" );
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

				else if ( token.Name == CommonSyntax.OpenBracketTokenName )
				{
					++IndentLevel;
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing {CommonSyntax.MetadataRegionTokenName} region" );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == CommonSyntax.MinimumVersionTokenName )
				{
					if ( HasParsedMinimumVersionToken )
					{
						throw new TokenException( $"{CommonSyntax.MetadataRegionTokenName} region cannot contain more than 1 {CommonSyntax.MinimumVersionTokenName} token" );
					}
					RejectUnsupportedVersions( token.Value );
					HasParsedMinimumVersionToken = true;
				}

				else
				{
					throw new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing {CommonSyntax.MetadataRegionTokenName} region" );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				throw new BracketMismatchException( $"A mismatched curly brace was found when parsing {CommonSyntax.MetadataRegionTokenName} region" );
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
				throw new TokenException( $"{CommonSyntax.MinimumVersionTokenName} token cannot be blank" );
			}

			if ( !AppVersion.IsVersionSupported( version ) )
			{
				throw new VersionNotFoundException( $"Input file version ({version}) is not supported by this version of {AppInfo.AppName}" );
			}
		}
	}
}
