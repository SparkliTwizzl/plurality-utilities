using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntryRegionParser : IEntryRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => ShortcutScriptGenerationSyntax.EntryRegionTokenName;


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

				else if ( token.Name == CommonSyntax.OpenBracketTokenName )
				{
					++IndentLevel;
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found when parsing region: {RegionName}" ) );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryColorTokenName )
				{
					if ( entry.Color != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryColorTokenName} token" ) );
					}
					entry.Color = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryDecorationTokenName )
				{
					if ( entry.Decoration != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryDecorationTokenName} token" ) );
					}
					entry.Decoration = token.Value;
				}
				
				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryIDTokenName )
				{
					if ( entry.ID != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryIDTokenName} token" ) );
					}
					entry.ID = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryNameTokenName )
				{
					entry.Identities.Add( ParseName( token.Value ) );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryLastNameTokenName )
				{
					if ( entry.LastIdentity != ScriptIdentity.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryLastNameTokenName} token" ) );
					}
					entry.LastIdentity = ParseName( token.Value );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryPronounTokenName )
				{
					if ( entry.Pronoun != string.Empty )
					{
						ExceptionLogger.LogAndThrow( new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryPronounTokenName} token" ) );
					}
					entry.Pronoun = token.Value;
				}

				else
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing region: {RegionName}" ) );
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
				ExceptionLogger.LogAndThrow( new FileRegionException( $"An {ShortcutScriptGenerationSyntax.EntryRegionTokenName} region did not contain all required fields" ) );
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
				ExceptionLogger.LogAndThrow( new TokenException( $"An invalid {ShortcutScriptGenerationSyntax.EntryNameTokenName} token was parsed" ) );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new ScriptIdentity( name, tag );
			return identity;
		}
	}
}
