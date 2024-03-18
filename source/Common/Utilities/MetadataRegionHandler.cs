using Petrichor.Common.Containers;


namespace Petrichor.Common.Utilities
{
	public class MetadataRegionHandler
	{
		public string VersionIsValidMessage { get; private set; } = $"Input file version is valid for { Info.AppInfo.AppNameAndVersion }";
		public RegionParser< StringWrapper > Parser { get; private set; }


		public MetadataRegionHandler()
		{
			var minimumVersionTokenHandler = ( string[] fileData, int regionStartIndex, StringWrapper result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var version = token.Value;
				Info.AppVersion.RejectUnsupportedVersions( version );
				return new RegionData< StringWrapper >()
				{
					Value = new( VersionIsValidMessage ),
				};
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
