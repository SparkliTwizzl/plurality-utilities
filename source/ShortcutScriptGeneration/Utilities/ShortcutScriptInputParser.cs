using Petrichor.Common.Enums;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptInputParser : IShortcutScriptInputParser
	{
		private const string entriesToken = "entries:";
		private const string metadataToken = "metadata:";
		private const string templatesToken = "templates:";

		private IShortcutScriptEntryParser EntryParser { get; set; }
		private IShortcutScriptMacroParser MacroParser { get; set; }
		private IShortcutScriptMetadataParser MetadataParser { get; set; }
		private IShortcutScriptTemplateParser TemplateParser { get; set; }


		public ShortcutScriptInputParser( IShortcutScriptMetadataParser metadataParser, IShortcutScriptEntryParser entryParser, IShortcutScriptTemplateParser templateParser, IShortcutScriptMacroParser macroParser )
		{
			EntryParser = entryParser;
			MacroParser = macroParser;
			MetadataParser = metadataParser;
			TemplateParser = templateParser;
		}


		public ShortcutScriptInput ParseInputFile( string inputFilePath )
		{
			var taskMessage = $"parsing input file \"{inputFilePath}\"";
			Log.TaskStarted( taskMessage );
			var data = ReadDataFromFile( inputFilePath );
			var input = ParseInputData( data );
			Log.TaskFinished( taskMessage );
			return input;
		}


		private ShortcutScriptInput ParseInputData( string[] data )
		{
			var input = new ShortcutScriptInput();
			var tokenParser = new StringTokenParser();
			var expectedTokens = new string[]
			{
				entriesToken,
				metadataToken,
				templatesToken,
			};

			for ( var i = 0 ; i < data.Length ; ++i )
			{
				var rawToken = data[ i ];
				var qualifiedToken = tokenParser.ParseToken( rawToken, expectedTokens );
				string? errorMessage;
				switch ( qualifiedToken.Qualifier )
				{
					case StringTokenQualifiers.Recognized:
					{
						if ( string.Compare( qualifiedToken.Value, entriesToken ) == 0 )
						{
							++i;
							input.Entries = EntryParser.ParseEntriesFromData( data, ref i );
						}
						else if ( string.Compare( qualifiedToken.Value, metadataToken ) == 0 )
						{
							++i;
							input.Metadata = MetadataParser.ParseMetadataFromData( data, ref i );
						}
						else if ( string.Compare( qualifiedToken.Value, templatesToken ) == 0 )
						{
							++i;
							input.Templates = TemplateParser.ParseTemplatesFromData( data, ref i );
						}
						if ( tokenParser.IndentLevel > 0 )
						{
							errorMessage = $"input file contains invalid data: a region was not closed properly when parsing token \"{qualifiedToken.Value}\"";
							Log.Error( errorMessage );
							throw new RegionNotClosedException( errorMessage );
						}
						break;
					}

					case StringTokenQualifiers.BlankLine:
					{
						break;
					}

					case StringTokenQualifiers.Unknown:
					default:
					{
						errorMessage = $"input file contains invalid data: an unknown token ( \"{qualifiedToken.Value}\" ) was read when a region name was expected";
						Log.Error( errorMessage );
						throw new UnknownTokenException( errorMessage );
					}
				}
			}

			input.Macros = MacroParser.GenerateMacrosFromInput( input );
			return input;
		}

		private static string[] ReadDataFromFile( string inputFilePath )
		{
			try
			{
				return File.ReadAllLines( inputFilePath );
			}
			catch ( Exception ex )
			{
				var errorMessage = "failed to read data from input file";
				Log.Error( errorMessage );
				throw new FileNotFoundException( errorMessage, ex );
			}
		}
	}
}
