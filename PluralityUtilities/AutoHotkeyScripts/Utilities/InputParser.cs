using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Common.Enums;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class InputParser
	{
		private const string EntriesToken = "entries:";
		private const string TemplatesToken = "templates:";


		public static Input ParseInputFile( string inputFilePath )
		{
			Log.WriteLineTimestamped( $"started parsing input file: { inputFilePath }");
			var data = ReadDataFromFile( inputFilePath );
			var input = ParseInputData( data );
			Log.WriteLineTimestamped( "finished parsing input file" );
			return input;
		}


		private static Input ParseInputData( string[] data )
		{
			var input = new Input();
			var tokenParser = new TokenParser();
			var expectedTokens = new string[]
			{
				EntriesToken,
				TemplatesToken,
			};

			for ( int i = 0; i < data.Length; ++i )
			{
				var rawToken = data[ i ];
				var qualifiedToken = tokenParser.ParseToken( rawToken, expectedTokens );
				switch ( qualifiedToken.Qualifier )
				{
					case TokenQualifiers.Recognized:
						if ( string.Compare( qualifiedToken.Token, EntriesToken ) == 0 )
						{
							++i;
							input.Entries = EntryParser.ParseEntriesFromData( data, ref i );
						}
						else if ( string.Compare( qualifiedToken.Token, TemplatesToken ) == 0 )
						{
							++i;
							input.Templates = TemplateParser.ParseTemplatesFromFile( data, ref i );
						}
						break;

					case TokenQualifiers.BlankLine:
						break;

					case TokenQualifiers.Unknown:
					default:
						var errorMessage = $"input file contains invalid data: an unknown Token ( \"{ qualifiedToken }\" ) was read when a region name was expected";
						Log.WriteLineTimestamped($"error: {errorMessage}");
						throw new UnknownTokenException(errorMessage);
				}
			}

			return input;
		}

		private static string[] ReadDataFromFile( string inputFilePath )
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
