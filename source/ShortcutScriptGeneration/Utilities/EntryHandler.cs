using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public static class EntryHandler
	{
		public static ProcessedRegionData<Entry> ColorTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.Color = token.Value;
			return new ProcessedRegionData<Entry>( result );
		}

		public static ProcessedRegionData<Entry> DecorationTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.Decoration = token.Value;
			return new ProcessedRegionData<Entry>( result );
		}

		public static ProcessedRegionData<Entry> IDTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.ID = token.Value;
			return new ProcessedRegionData<Entry>( result );
		}

		public static ProcessedRegionData<Entry> NameTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.Identities.Add( ParseNameTokenValue( token ) );
			return new ProcessedRegionData<Entry>( result );
		}

		public static ProcessedRegionData<Entry> LastNameTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.LastIdentity = ParseNameTokenValue( token );
			return new ProcessedRegionData<Entry>( result );
		}

		public static ProcessedRegionData<Entry> PronounTokenHandler( IndexedString[] regionData, int tokenStartIndex, Entry result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			result.Pronoun = token.Value;
			return new ProcessedRegionData<Entry>( result );
		}


		private static Identity ParseNameTokenValue( StringToken token )
		{
			var components = token.Value.Split( '@' );
			if ( components.Length != 2 )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A {token.Key} token had an invalid value ( \"{token.Value}\" )." ), token.LineNumber );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new Identity( name, tag );
			return identity;
		}
	}
}
