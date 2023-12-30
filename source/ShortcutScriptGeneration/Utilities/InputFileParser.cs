using Petrichor.Common.Enums;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser : IShortcutScriptInputParser
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }


		public InputFileParser( IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;
		}


		public ShortcutScriptInput Parse( string filePath )
		{
			var taskMessage = $"parsing input file \"{filePath}\"";
			Log.TaskStarted( taskMessage );
			var data = ReadFileData( filePath );
			var input = ParseData( data );
			Log.TaskFinished( taskMessage );
			return input;
		}


		private ShortcutScriptInput ParseData( string[] data )
		{
			var input = new ShortcutScriptInput();
			var tokenParser = new StringTokenParser();
			var expectedTokens = new string[]
			{
				ShortcutScriptGenerationSyntax.EntriesRegionToken,
				ShortcutScriptGenerationSyntax.ModuleOptionsRegionToken,
				ShortcutScriptGenerationSyntax.TemplatesRegionToken,
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
						if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.EntriesRegionToken )
						{
							++i;
							input.Entries = EntriesRegionParser.ParseEntriesFromData( data, ref i );
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.ModuleOptionsRegionToken )
						{
							++i;
							input.ModuleOptions = ModuleOptionsRegionParser.ParseModuleOptionsFromData( data, ref i );
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.TemplatesRegionToken )
						{
							++i;
							input.Templates = TemplatesRegionParser.ParseTemplatesFromData( data, ref i );
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

			input.Macros = MacroGenerator.Generate( input );
			return input;
		}

		private static string[] ReadFileData( string filePath )
		{
			try
			{
				return File.ReadAllLines( filePath );
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
