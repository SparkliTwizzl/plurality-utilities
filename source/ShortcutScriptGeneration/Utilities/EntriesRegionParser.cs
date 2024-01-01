using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntriesRegionParser : IEntriesRegionParser
	{
		private IEntryParser EntryParser { get; set; }
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => ShortcutScriptGenerationSyntax.EntriesRegionTokenName;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;


		public EntriesRegionParser( IEntryParser entryParser ) => EntryParser = entryParser;


		public ScriptEntry[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {RegionName} regions" );
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

				else if ( token.Name == CommonSyntax.OpenBracketTokenName )
				{
					++IndentLevel;
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						throw new BracketMismatchException( $"A mismatched close bracket was found when parsing region: {RegionName}" );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryTokenName )
				{
					var regionDataTrimmedToEntry = regionData[ ( i + 1 ).. ];
					var entry = EntryParser.Parse( regionDataTrimmedToEntry );
					entries.Add( entry );
					i += EntryParser.LinesParsed;
				}

				else
				{
					throw new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing region: {RegionName}" );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				throw new BracketMismatchException( $"A mismatched open bracket was found when parsing region: {RegionName}" );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.Info( $"Parsed {entries.Count} entries" );
			Log.TaskFinish( taskMessage );
			return entries.ToArray();
		}
	}
}
