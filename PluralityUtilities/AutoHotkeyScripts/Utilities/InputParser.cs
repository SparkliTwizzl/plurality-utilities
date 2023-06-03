using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Common.Enums;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class InputParser
	{
		private const string EntriesToken = "entries:";
		private const string TemplatesToken = "templates:";
		private EntryParser EntryParser { get; set; }


		public InputParser( EntryParser entryParser )
		{
			EntryParser = entryParser;
		}


		public Input ParseInputFile( string inputFilePath )
		{
			Log.WriteLineTimestamped( $"started parsing input file: { inputFilePath }");
			var data = ReadDataFromFile( inputFilePath );
			var input = ParseInputData( data );
			Log.WriteLineTimestamped( "finished parsing input file" );
			return input;
		}


		private Input ParseInputData( string[] data )
		{
			var input = new Input();
			var tokenParser = new TokenParser();
			var expectedTokens = new string[]
			{
				EntriesToken,
				TemplatesToken,
			};

			for ( var i = 0; i < data.Length; ++i )
			{
				var rawToken = data[ i ];
				var qualifiedToken = tokenParser.ParseToken( rawToken, expectedTokens );
				string? errorMessage;
				switch ( qualifiedToken.Qualifier )
				{
					case TokenQualifiers.Recognized:
						if ( string.Compare( qualifiedToken.Value, EntriesToken ) == 0 )
						{
							++i;
							input.Entries = EntryParser.ParseEntriesFromData( data, ref i );
						}
						else if ( string.Compare( qualifiedToken.Value, TemplatesToken ) == 0 )
						{
							++i;
							input.Templates = TemplateParser.ParseTemplatesFromData( data, ref i );
						}
						if ( tokenParser.IndentLevel > 0 )
						{
							errorMessage = $"input file contains invalid data: a region was not closed properly when parsing token \"{ qualifiedToken.Value }\"";
							Log.WriteLineTimestamped($"error: {errorMessage}");
							throw new RegionNotClosedException(errorMessage);
						}
						break;

					case TokenQualifiers.BlankLine:
						break;

					case TokenQualifiers.Unknown:
					default:
						errorMessage = $"input file contains invalid data: an unknown token ( \"{ qualifiedToken.Value }\" ) was read when a region name was expected";
						Log.WriteLineTimestamped($"error: {errorMessage}");
						throw new UnknownTokenException(errorMessage);
				}
			}

			return input;
		}

		private string[] ReadDataFromFile( string inputFilePath )
		{
			try
			{
				var data = File.ReadAllLines( inputFilePath );
				Log.WriteLineTimestamped( "successfully read data from input file" );
				return data;
			}
			catch ( Exception ex )
			{
				var errorMessage = "failed to read data from input file";
				Log.WriteLineTimestamped( $"error: { errorMessage }; { ex.Message }" );
				throw new FileNotFoundException( errorMessage, ex );
			}
		}
	}
}
