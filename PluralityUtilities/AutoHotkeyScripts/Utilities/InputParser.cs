using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class InputParser
	{
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
			var result = new Input();
			for ( int i = 0; i < data.Length; ++i )
			{
				var token = data[ i ];
				if ( string.Compare( token, "entries:" ) == 0 )
				{
					++i;
					result.Entries = EntryParser.ParseEntriesFromData( data, ref i );
				}
				else if ( string.Compare( token, "templates:" ) == 0 )
				{
					++i;
					result.Templates = TemplateParser.ParseTemplatesFromFile( data, ref i );
				}
				else if (string.Compare(token, "") == 0) // ignore blank lines
				{
					continue;
				}
				else
				{
					var errorMessage = $"input file contains invalid data: an unknown token ( \"{ token }\" ) was read when a region name was expected";
					Log.WriteLineTimestamped($"error: {errorMessage}");
					throw new UnknownTokenException(errorMessage);
				}
			}
			return result;
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
