using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;


namespace Petrichor.Common.Utilities
{
	public class MetadataRegionHandler
	{
		public bool HasParsedMinimumVersionToken { get; private set; } = false;
		public RegionParser< StringWrapper > Parser { get; private set; }


		public MetadataRegionHandler()
		{
			var minimumVersionTokenHandler = ( string[] fileData, int regionStartIndex, StringWrapper result ) =>
			{
				if ( HasParsedMinimumVersionToken )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"Region cannot contain more than { Info.TokenMetadata.MaxMinimumVersionTokens } { Syntax.TokenNames.MinimumVersion } token" ) );
				}

				var token = new StringToken( fileData[ regionStartIndex ] );
				var version = token.Value;
				Info.AppVersion.RejectUnsupportedVersions( version );

				HasParsedMinimumVersionToken = true;
				return new RegionData< StringWrapper >();
			};

			var tokenHandlers = new Dictionary< string, Func< string[], int, StringWrapper, RegionData< StringWrapper > > >()
			{
				{ Syntax.TokenNames.MinimumVersion, minimumVersionTokenHandler },
			};

			var parserDescriptor = new RegionParserDescriptor< StringWrapper >()
			{
				RegionName = Syntax.TokenNames.MetadataRegion,
				MaxRegionsAllowed = Info.TokenMetadata.MaxMetadataRegions,
				MinRegionsRequired = Info.TokenMetadata.MinMetadataRegions,
				TokenHandlers = tokenHandlers,
				MaxAllowedTokenInstances = new()
				{
					{ Syntax.TokenNames.MinimumVersion, Info.TokenMetadata.MaxMinimumVersionTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ Syntax.TokenNames.MinimumVersion, Info.TokenMetadata.MinMinimumVersionTokens },
				},
			};

			Parser = new RegionParser< StringWrapper >( parserDescriptor );
		}
	}
}
