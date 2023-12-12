using Petrichor.AutoHotkeyScripts.Containers;
using Petrichor.AutoHotkeyScripts.Exceptions;
using Petrichor.Common.Enums;
using Petrichor.Logging;


namespace Petrichor.AutoHotkeyScripts.Utilities
{
	public class InputParser
	{
		private const string EntriesToken = "entries:";
		private const string TemplatesToken = "templates:";
		private EntryParser EntryParser { get; set; }
		private TemplateParser TemplateParser { get; set; }


		public InputParser( EntryParser entryParser, TemplateParser templateParser )
		{
			EntryParser = entryParser;
			TemplateParser = templateParser;
		}


		public Input ParseInputFile( string inputFilePath )
		{
			Log.TaskStarted( $"parsing input file: { inputFilePath }");
			var data = ReadDataFromFile( inputFilePath );
			var input = ParseInputData( data );
			Log.TaskFinished( "parsing input file" );
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
						{
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
								Log.Error( errorMessage );
								throw new RegionNotClosedException( errorMessage );
							}
							break;
						}

					case TokenQualifiers.BlankLine:
						{
							break;
						}

					case TokenQualifiers.Unknown:
					default:
						{
							errorMessage = $"input file contains invalid data: an unknown token ( \"{ qualifiedToken.Value }\" ) was read when a region name was expected";
							Log.Error( errorMessage );
							throw new UnknownTokenException( errorMessage );
						}
				}
			}

			return input;
		}

		private string[] ReadDataFromFile( string inputFilePath )
		{
			try
			{
				var data = File.ReadAllLines( inputFilePath );
				Log.Info( "successfully read data from input file" );
				return data;
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
