using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Enums;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class EntryParser
	{
		private static int IndentLevel { get; set; } = 0;


		/// <summary>
		/// throws BlankInputFieldException if file contains a field with no value<para/>
		/// throws DuplicateInputFieldException if file contains an entry with more than one decoration field<para/>
		/// throws DuplicateInputFieldException if file contains an entry with more than one pronoun field<para/>
		/// throws InputEntryNotClosedException if file contains an entry that is not closed<para/>
		/// throws FileNotFoundException if file data could not be read<para/>
		/// throws InvalidInputFieldException if file contains a tag field with spaces in it<para/>
		/// throws MissingInputFieldException if file contains an entry with no identity fields<para/>
		/// throws MissingInputFieldException if file contains an identity field with no name field<para/>
		/// throws MissingInputFieldException if file contains an identity field with no tag field<para/>
		/// throws UnexpectedCharacterException if file contains a line that starts with an unexpected character<para/>
		/// </summary>
		/// <param name="inputFilePath">path to input file to be parsed</param>
		/// <returns>parsed input data</returns>
		public static Entry[] ParseEntriesFromData( string[] data, ref int i )
		{
			Log.WriteLineTimestamped( "started parsing entries from input data");
			var entries = new List< Entry >();
			for ( ; i < data.Length; ++i )
			{
				var token = data[i].Trim();
				if ( string.Compare( token, "{" ) == 0 )
				{
					++IndentLevel;
					if ( IndentLevel > 1 )
					{
						++i;
						var entry = ParseEntry( data, ref i );
						entries.Add( entry );
						Log.WriteTimestamped( "successfully parsed entry: names/tags [" );
						foreach ( Identity identity in entry.Identities )
						{
							Log.Write( $"{ identity.Name }/{ identity.Tag }, " );
						}
						Log.WriteLine( $"], pronoun [{ entry.Pronoun }], decoration [{ entry.Decoration }]" );
					}
				}
				else if ( string.Compare( token, "}" ) == 0 )
				{
					--IndentLevel;
					if ( IndentLevel == 0 )
					{
						break;
					}
				}
				else if ( string.Compare( token, "" ) == 0 ) // ignore blank lines
				{
					continue;
				}
				else
				{
					var errorMessage = $"input file contains invalid data: an unexpected character ( \"{ token }\" ) was read when '{{' was expected";
					Log.WriteLineTimestamped( $"error: { errorMessage }" );
					throw new UnexpectedCharacterException( errorMessage );
				}
			}
			Log.WriteLineTimestamped( "finished parsing entries from input data" );
			return entries.ToArray();
		}


		private static void ParseDecoration( string line, ref Entry entry )
		{
			if ( entry.Decoration != string.Empty )
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one decoration field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new DuplicateInputFieldException( errorMessage );
			}
			if ( line.Length < 2 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank decoration field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new BlankInputFieldException( errorMessage );
			}
			entry.Decoration = line.Substring( 1, line.Length - 1 );
		}

		private static void ParseIdentity( string line, ref Entry entry )
		{
			Identity identity = new Identity();
			ParseName( line, ref identity );
			ParseTag( line, ref identity );
			entry.Identities.Add( identity );
		}

		private static LineTypes ParseLine( string line, ref Entry entry )
		{
			line = line.TrimStart();
			var firstChar = line[ 0 ];
			switch ( firstChar )
			{
				case '}':
					--IndentLevel;
					return LineTypes.EntryEnd;
				case '%':
					ParseIdentity( line, ref entry );
					return LineTypes.Name;
				case '$':
					ParsePronoun( line, ref entry );
					return LineTypes.Pronoun;
				case '&':
					ParseDecoration( line, ref entry );
					return LineTypes.Decoration;
				default:
					return LineTypes.Unknown;
			}
		}

		private static void ParseName( string line, ref Identity identity )
		{
			var fieldStart = line.IndexOf( '#' );
			var fieldEnd = line.LastIndexOf( '#' );
			if ( fieldStart < 0 )
			{
				var errorMessage = "input file contains invalid data: an entry had no name fields";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new MissingInputFieldException( errorMessage );
			}
			var name = line.Substring( fieldStart + 1, fieldEnd - ( fieldStart + 1 ) );
			if ( name.Length < 1 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new BlankInputFieldException( errorMessage );
			}
			identity.Name = name;
		}

		private static Entry ParseEntry( string[] data, ref int index )
		{
			Log.WriteLineTimestamped( "started parsing next entry" );
			var entry = new Entry();
			for ( ; index < data.Length; ++index )
			{
				var line = data[ index ];
				var result = ParseLine( line, ref entry );
				switch ( result )
				{
					case LineTypes.EntryEnd:
						if ( entry.Identities.Count < 1 )
						{
							var noIdentities = "input file contains invalid data: an entry did not contain any identity fields";
							Log.WriteLineTimestamped( $"error: { noIdentities }" );
							throw new MissingInputFieldException( noIdentities );
						}
						return entry;
					case LineTypes.Unknown:
						var unexpectedChar = line.Trim()[ 0 ];
						var errorMessage = $"input file contains invalid data: a line started with a character ( \"{ unexpectedChar }\" ) that was not expected at this time";
						Log.WriteLineTimestamped( $"error: { errorMessage }" );
						throw new UnexpectedCharacterException( errorMessage );
					default:
						break;
				}
			}
			var lastEntryNotClosed = "input file contains invalid data: last entry was not closed";
			Log.WriteLineTimestamped( $"error: { lastEntryNotClosed }" );
			throw new InputEntryNotClosedException( lastEntryNotClosed );
		}

		private static void ParsePronoun( string line, ref Entry entry )
		{
			if ( entry.Pronoun != string.Empty )
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new DuplicateInputFieldException( errorMessage );
			}
			if ( line.Length < 2 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank pronoun field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new BlankInputFieldException( errorMessage );
			}
			entry.Pronoun = line.Substring( 1, line.Length - 1 );
		}

		private static void ParseTag( string line, ref Identity identity )
		{
			var fieldStart = line.IndexOf( '@' );
			if ( fieldStart < 0 )
			{
				var errorMessage = "input file contains invalid data: an entry contained an identity field without a tag field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new MissingInputFieldException( errorMessage );
			}
			var lastSpace = line.LastIndexOf( ' ' );
			if ( lastSpace >= fieldStart )
			{
				var errorMessage = "input file contains invalid data: tag fields cannot contain spaces";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new InvalidInputFieldException( errorMessage );
			}
			var tag = line.Substring( fieldStart + 1, line.Length - ( fieldStart + 1 ) );
			if ( tag.Length < 1 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank tag field";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new BlankInputFieldException( errorMessage );
			}
			identity.Tag = tag;
		}
	}
}
