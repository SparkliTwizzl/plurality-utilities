using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntriesRegionParser : IEntriesRegionParser
	{
		private IEntryRegionParser EntryParser { get; set; }
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => TokenNames.EntriesRegion;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed => 1;
		public int RegionsParsed { get; private set; } = 0;


		public EntriesRegionParser( IEntryRegionParser entryParser ) => EntryParser = entryParser;


		public ScriptEntry[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than { MaxRegionsAllowed } { RegionName } regions" ) );
			}

			var entries = new List<ScriptEntry>();
			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );
				var isParsingFinished = false;

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == Common.Syntax.TokenNames.RegionOpen )
				{
					++IndentLevel;
				}

				else if ( token.Name == Common.Syntax.TokenNames.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched close bracket was found when parsing region: { RegionName }" ) );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == TokenNames.EntryRegion )
				{
					var dataTrimmedToRegion = regionData[ ( i + 1 ).. ];
					var entry = EntryParser.Parse( dataTrimmedToRegion );
					entries.Add( entry );
					LinesParsed += EntryParser.LinesParsed;
					i += EntryParser.LinesParsed;
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
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched open bracket was found when parsing region: {RegionName}" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.Info( $"Parsed {entries.Count} entries" );
			Log.TaskFinish( taskMessage );
			return entries.ToArray();
		}
	}
}
