using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
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
		public int MaxRegionsAllowed { get; private set; } = int.MaxValue;
		public int RegionsParsed { get; private set; } = 0;


		public ScriptEntry Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {RegionName} regions" );
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
						throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing region: {RegionName}" );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryDecorationTokenName )
				{
					if ( entry.Decoration != string.Empty )
					{
						throw new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryDecorationTokenName} token" );
					}
					entry.Decoration = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryNameTokenName )
				{
					entry.Identities.Add( ParseName( token.Value ) );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntryPronounTokenName )
				{
					if ( entry.Pronoun != string.Empty )
					{
						throw new TokenException( $"Entries cannot contain more than 1 {ShortcutScriptGenerationSyntax.EntryPronounTokenName} token" );
					}
					entry.Pronoun = token.Value;
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
				throw new BracketMismatchException( $"A mismatched curly brace was found when parsing region: {RegionName}" );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinish( taskMessage );
			return entry;
		}

		private ShortcutScriptIdentity ParseName( string token )
		{
			var components = token.Split( '@' );
			if ( components.Length != 2 )
			{
				throw new TokenException( $"An invalid {ShortcutScriptGenerationSyntax.EntryNameTokenName} token was parsed" );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new ShortcutScriptIdentity( name, tag );
			return identity;
		}
	}
}
