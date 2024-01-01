using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class EntryParser : IEntryParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => ShortcutScriptGenerationSyntax.EntryRegionTokenName;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = int.MaxValue;
		public int RegionsParsed { get; private set; } = 0;


		public ScriptEntry Parse( string[] regionData )
		{
			return new ScriptEntry();
		}


		//private static ShortcutScriptEntryLineTypes IdentifyLineType( string line )
		//{
		//	var firstChar = line[ 0 ];
		//	switch ( firstChar )
		//	{
		//		case decorationLineChar:
		//		{
		//			return ShortcutScriptEntryLineTypes.Decoration;
		//		}

		//		case entryStartLineChar:
		//		{
		//			return ShortcutScriptEntryLineTypes.EntryStart;
		//		}

		//		case entryEndLineChar:
		//		{
		//			return ShortcutScriptEntryLineTypes.EntryEnd;
		//		}

		//		case identityLineChar:
		//		{
		//			return ShortcutScriptEntryLineTypes.Identity;
		//		}

		//		case pronounLineChar:
		//		{
		//			return ShortcutScriptEntryLineTypes.Pronoun;
		//		}

		//		default:
		//		{
		//			return ShortcutScriptEntryLineTypes.Unknown;
		//		}
		//	}
		//}

		//private static void ParseDecoration( string line, ref ScriptEntry entry )
		//{
		//	if ( entry.Decoration != string.Empty )
		//	{
		//		throw new DuplicateInputFieldException( "An entry contains more than one decoration field" );
		//	}
		//	if ( line.Length < 2 )
		//	{
		//		throw new BlankInputFieldException( "An entry contains a blank decoration field" );
		//	}
		//	entry.Decoration = line[ 1.. ];
		//}

		//private static void ParseIdentity( string line, ref ScriptEntry entry )
		//{
		//	var identity = new ShortcutScriptIdentity();
		//	ParseName( line, ref identity );
		//	ParseTag( line, ref identity );
		//	entry.Identities.Add( identity );
		//}

		//private static void ParseName( string line, ref ShortcutScriptIdentity identity )
		//{
		//	var fieldStart = 0;

		//	var fieldEnd = line.IndexOf( '@' );
		//	if ( fieldEnd <= fieldStart )
		//	{
		//		throw new InvalidInputFieldException( "An entry contains an invalid name field" );
		//	}

		//	var nameStart = fieldStart + 1;
		//	var nameEnd = fieldEnd;

		//	var name = line[ nameStart..nameEnd ].Trim();
		//	if ( name.Length < 1 )
		//	{
		//		throw new BlankInputFieldException( "An entry contains a blank name field" );
		//	}

		//	identity.Name = name;
		//}

		//private static ScriptEntry ParseEntry( string[] data, ref int i )
		//{
		//	var entry = new ScriptEntry();
		//	for ( ; i < data.Length ; ++i )
		//	{
		//		var line = data[ i ].Trim();

		//		var isLineBlank = line == string.Empty;
		//		var isLineComment = line.IndexOf( CommonSyntax.LineCommentToken ) == 0;
		//		if ( isLineBlank || isLineComment )
		//		{
		//			break;
		//		}

		//		var lineType = IdentifyLineType( line );
		//		switch ( lineType )
		//		{
		//			case ShortcutScriptEntryLineTypes.EntryEnd:
		//			{
		//				if ( entry.Identities.Count < 1 )
		//				{
		//					throw new MissingInputFieldException( $"parsing entries failed at token #{i} :: input file contains invalid data: an entry did not contain any identity fields" );
		//				}
		//				--TokenParser.IndentLevel;
		//				return entry;
		//			}

		//			case ShortcutScriptEntryLineTypes.Identity:
		//			{
		//				ParseIdentity( line, ref entry );
		//				break;
		//			}

		//			case ShortcutScriptEntryLineTypes.Pronoun:
		//			{
		//				ParsePronoun( line, ref entry );
		//				break;
		//			}

		//			case ShortcutScriptEntryLineTypes.Decoration:
		//			{
		//				ParseDecoration( line, ref entry );
		//				break;
		//			}

		//			case ShortcutScriptEntryLineTypes.Unknown:
		//			{
		//				var unexpectedChar = line[ 0 ];
		//				throw new UnexpectedCharacterException( $"parsing entries failed at token #{i} :: input file contains invalid data: a line started with a character ( \"{unexpectedChar}\" ) that was not expected at this time" );
		//			}
		//		}
		//	}
		//	throw new InputEntryNotClosedException( "Last entry was not closed" );
		//}

		//private static void ParsePronoun( string line, ref ScriptEntry entry )
		//{
		//	if ( entry.Pronoun != string.Empty )
		//	{
		//		throw new DuplicateInputFieldException( "An entry contains more than one pronoun field" );
		//	}
		//	if ( line.Length < 2 )
		//	{
		//		throw new BlankInputFieldException( "An entry contains a blank pronoun field" );
		//	}
		//	entry.Pronoun = line[ 1.. ];
		//}

		//private static void ParseTag( string line, ref ShortcutScriptIdentity identity )
		//{
		//	var fieldStart = line.IndexOf( '@' );
		//	if ( fieldStart < 0 )
		//	{
		//		throw new MissingInputFieldException( "An entry contains an identity field without a tag field" );
		//	}

		//	var tagStart = fieldStart + 1;
		//	var trimmedLine = line[ tagStart.. ];

		//	var firstSpace = trimmedLine.IndexOf( ' ' );
		//	var tagEnd = firstSpace > -1 ? firstSpace : trimmedLine.Length;
		//	var tag = trimmedLine[ ..tagEnd ];
		//	if ( tag.Length < 1 )
		//	{
		//		throw new BlankInputFieldException( "An entry contains a blank tag field" );
		//	}

		//	identity.Tag = tag;
		//}
	}
}
