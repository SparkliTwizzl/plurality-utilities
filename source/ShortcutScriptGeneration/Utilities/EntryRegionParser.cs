using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntryRegionParser : IEntryRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => TokenNames.EntryRegion;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed => int.MaxValue;
		public int RegionsParsed { get; private set; } = 0;


		public ScriptEntry Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {RegionName} regions" ) );
			}

			var entry = new ScriptEntry();
			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var isParsingFinished = false;
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == Common.Syntax.Tokens.RegionOpen )
				{
					++IndentLevel;
				}

				else if ( token.Name == Common.Syntax.Tokens.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found when parsing region: { RegionName }" ) );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == TokenNames.EntryColor )
				{
					if ( entry.Color != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 { Tokens.EntryColor } token" ) );
					}
					entry.Color = token.Value;
				}

				else if ( token.Name == TokenNames.EntryDecoration )
				{
					if ( entry.Decoration != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 { Tokens.EntryDecoration } token" ) );
					}
					entry.Decoration = token.Value;
				}
				
				else if ( token.Name == TokenNames.EntryID )
				{
					if ( entry.ID != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 { Tokens.EntryID } token" ) );
					}
					entry.ID = token.Value;
				}

				else if ( token.Name == TokenNames.EntryName )
				{
					entry.Identities.Add( ParseName( token.Value ) );
				}

				else if ( token.Name == TokenNames.EntryLastName )
				{
					if ( entry.LastIdentity != ScriptIdentity.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 { Tokens.EntryLastName } token" ) );
					}
					entry.LastIdentity = ParseName( token.Value );
				}

				else if ( token.Name == TokenNames.EntryPronoun )
				{
					if ( entry.Pronoun != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 { Tokens.EntryPronoun } token" ) );
					}
					entry.Pronoun = token.Value;
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
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found when parsing region: {RegionName}" ) );
			}

			var entryHasRequiredValues = entry.ID != string.Empty && entry.Identities.Count > 0;
			if ( !entryHasRequiredValues )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"An { Tokens.EntryRegion } region did not contain all required fields" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinish( taskMessage );
			return entry;
		}

		private static ScriptIdentity ParseName( string token )
		{
			var components = token.Split( '@' );
			if ( components.Length != 2 )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"An invalid { Tokens.EntryName } token was parsed" ) );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new ScriptIdentity( name, tag );
			return identity;
		}
	}
}
