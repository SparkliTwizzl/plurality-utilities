using Petrichor.Common.Enums;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Enums;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntriesRegionParser : IEntriesRegionParser
	{
		private const char decorationLineChar = '&';
		private const char entryEndLineChar = '}';
		private const char entryStartLineChar = '{';
		private const char identityLineChar = '#';
		private const char pronounLineChar = '$';

		private StringTokenParser TokenParser { get; set; } = new StringTokenParser();


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;


		public EntriesRegionParser() { }


		public ScriptEntry[] Parse( string[] regionData, ref int i )
		{
			var taskMessage = $"parsing {ShortcutScriptGenerationSyntax.EntriesRegionTokenName} region data";
			Log.TaskStarted( taskMessage );
			
			string? errorMessage;

			if ( HasParsedMaxAllowedRegions )
			{
				errorMessage = $"input file cannot contain more than {MaxRegionsAllowed} {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} regions";
				Log.Error( errorMessage );
				throw new FileRegionException( errorMessage );
			}

			var entries = new List<ScriptEntry>();
			var expectedTokens = new string[]
			{
				decorationLineChar.ToString(),
				identityLineChar.ToString(),
				pronounLineChar.ToString(),
			};

			for ( ; i < regionData.Length ; ++i )
			{
				var isParsingFinished = false;
				var line = regionData[ i ].Trim();

				var isLineBlank = line.Length < 1;
				var isLineComment = line.IndexOf( CommonSyntax.LineCommentToken ) == 0;
				if ( isLineBlank || isLineComment )
				{
					continue;
				}

				var firstChar = line.FirstOrDefault();
				var token = TokenParser.ParseToken( firstChar.ToString(), expectedTokens );
				switch ( token.Qualifier )
				{
					case StringTokenQualifiers.CloseBracket:
					{
						if ( TokenParser.IndentLevel == 0 )
						{
							isParsingFinished = true;
						}
						break;
					}

					case StringTokenQualifiers.Unknown:
					{
						errorMessage = $"parsing entries failed at token # {i} :: input file contains invalid data: a line started with a character ( \"{firstChar}\" ) that was not expected at this time";
						Log.Error( errorMessage );
						throw new UnexpectedCharacterException( errorMessage );
					}

					default:
					{
						if ( TokenParser.IndentLevel > 1 )
						{
							++i;
							var entry = ParseEntry( regionData, ref i );
							entries.Add( entry );
							Log.WriteWithTimestamp( "parsed entry: names/tags [" );
							foreach ( var identity in entry.Identities )
							{
								Log.Write( $"{identity.Name}/{identity.Tag}, " );
							}
							Log.WriteLine( $"], pronoun [{entry.Pronoun}], decoration [{entry.Decoration}]" );
						}
						break;
					}
				}
				if ( isParsingFinished )
				{
					break;
				}
			}
			if ( TokenParser.IndentLevel > 0 )
			{
				errorMessage = "input file contains invalid data: an entry was not closed";
				Log.Error( errorMessage );
				throw new InputEntryNotClosedException( errorMessage );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinished( taskMessage );
			return entries.ToArray();
		}


		private static ShortcutScriptEntryLineTypes IdentifyLineType( string line )
		{
			var firstChar = line[ 0 ];
			switch ( firstChar )
			{
				case decorationLineChar:
				{
					return ShortcutScriptEntryLineTypes.Decoration;
				}

				case entryStartLineChar:
				{
					return ShortcutScriptEntryLineTypes.EntryStart;
				}

				case entryEndLineChar:
				{
					return ShortcutScriptEntryLineTypes.EntryEnd;
				}

				case identityLineChar:
				{
					return ShortcutScriptEntryLineTypes.Identity;
				}

				case pronounLineChar:
				{
					return ShortcutScriptEntryLineTypes.Pronoun;
				}

				default:
				{
					return ShortcutScriptEntryLineTypes.Unknown;
				}
			}
		}

		private static void ParseDecoration( string line, ref ScriptEntry entry )
		{
			if ( entry.Decoration != string.Empty )
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one decoration field";
				Log.Error( errorMessage );
				throw new DuplicateInputFieldException( errorMessage );
			}
			if ( line.Length < 2 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank decoration field";
				Log.Error( errorMessage );
				throw new BlankInputFieldException( errorMessage );
			}
			entry.Decoration = line[ 1.. ];
		}

		private static void ParseIdentity( string line, ref ScriptEntry entry )
		{
			var identity = new ShortcutScriptIdentity();
			ParseName( line, ref identity );
			ParseTag( line, ref identity );
			entry.Identities.Add( identity );
		}

		private static void ParseName( string line, ref ShortcutScriptIdentity identity )
		{
			var fieldStart = 0;

			var fieldEnd = line.IndexOf( '@' );
			if ( fieldEnd <= fieldStart )
			{
				var errorMessage = "input file contains invalid data: an entry had an invalid name field";
				Log.Error( errorMessage );
				throw new InvalidInputFieldException( errorMessage );
			}

			var nameStart = fieldStart + 1;
			var nameEnd = fieldEnd;

			var name = line[ nameStart..nameEnd ].Trim();
			if ( name.Length < 1 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.Error( errorMessage );
				throw new BlankInputFieldException( errorMessage );
			}

			identity.Name = name;
		}

		private ScriptEntry ParseEntry( string[] data, ref int i )
		{
			var taskMessage = "parsing entry";
			Log.TaskStarted( taskMessage );

			var entry = new ScriptEntry();
			string? errorMessage;
			for ( ; i < data.Length ; ++i )
			{
				var line = data[ i ].Trim();

				var isLineBlank = line == string.Empty;
				var isLineComment = line.IndexOf( CommonSyntax.LineCommentToken ) == 0;
				if ( isLineBlank || isLineComment )
				{
					break;
				}

				var lineType = IdentifyLineType( line );
				switch ( lineType )
				{
					case ShortcutScriptEntryLineTypes.EntryEnd:
					{
						if ( entry.Identities.Count < 1 )
						{
							errorMessage = $"parsing entries failed at token #{i} :: input file contains invalid data: an entry did not contain any identity fields";
							Log.Error( errorMessage );
							throw new MissingInputFieldException( errorMessage );
						}
						--TokenParser.IndentLevel;
						Log.TaskFinished( taskMessage );
						return entry;
					}

					case ShortcutScriptEntryLineTypes.Identity:
					{
						ParseIdentity( line, ref entry );
						break;
					}

					case ShortcutScriptEntryLineTypes.Pronoun:
					{
						ParsePronoun( line, ref entry );
						break;
					}

					case ShortcutScriptEntryLineTypes.Decoration:
					{
						ParseDecoration( line, ref entry );
						break;
					}

					case ShortcutScriptEntryLineTypes.Unknown:
					{
						var unexpectedChar = line[ 0 ];
						errorMessage = $"parsing entries failed at token #{i} :: input file contains invalid data: a line started with a character ( \"{unexpectedChar}\" ) that was not expected at this time";
						Log.Error( errorMessage );
						throw new UnexpectedCharacterException( errorMessage );
					}
				}
			}
			errorMessage = "input file contains invalid data: last entry was not closed";
			Log.Error( errorMessage );
			throw new InputEntryNotClosedException( errorMessage );
		}

		private static void ParsePronoun( string line, ref ScriptEntry entry )
		{
			if ( entry.Pronoun != string.Empty )
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun field";
				Log.Error( errorMessage );
				throw new DuplicateInputFieldException( errorMessage );
			}
			if ( line.Length < 2 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank pronoun field";
				Log.Error( errorMessage );
				throw new BlankInputFieldException( errorMessage );
			}
			entry.Pronoun = line[ 1.. ];
		}

		private static void ParseTag( string line, ref ShortcutScriptIdentity identity )
		{
			var fieldStart = line.IndexOf( '@' );
			if ( fieldStart < 0 )
			{
				var errorMessage = "input file contains invalid data: an entry contained an identity field without a tag field";
				Log.Error( errorMessage );
				throw new MissingInputFieldException( errorMessage );
			}

			var tagStart = fieldStart + 1;
			var trimmedLine = line[ tagStart.. ];

			var firstSpace = trimmedLine.IndexOf( ' ' );
			var tagEnd = firstSpace > -1 ? firstSpace : trimmedLine.Length;
			var tag = trimmedLine[ ..tagEnd ];
			if ( tag.Length < 1 )
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank tag field";
				Log.Error( errorMessage );
				throw new BlankInputFieldException( errorMessage );
			}

			identity.Tag = tag;
		}

		ScriptEntry IRegionParser<ScriptEntry>.Parse( string[] regionData, ref int i ) => throw new NotImplementedException();
	}
}
