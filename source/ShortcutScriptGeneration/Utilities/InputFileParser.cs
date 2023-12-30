using Petrichor.Common.Enums;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
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


		public ScriptInput Parse( string filePath )
		{
			var taskMessage = $"Parse input file \"{filePath}\"";
			Log.TaskStart( taskMessage );
			var data = ReadFileData( filePath );
			var input = ParseData( data );
			Log.TaskFinish( taskMessage );
			return input;
		}


		private ScriptInput ParseData( string[] data )
		{
			var input = new ScriptInput();
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
				switch ( qualifiedToken.Qualifier )
				{
					case StringTokenQualifiers.Recognized:
					{
						if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.EntriesRegionToken )
						{
							++i;
							var dataTrimmedToEntries = data[ i.. ];
							input.Entries = EntriesRegionParser.Parse( dataTrimmedToEntries );
							i += EntriesRegionParser.LinesParsed;
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.ModuleOptionsRegionToken )
						{
							++i;
							var dataTrimmedToModuleOptions = data[ i.. ];
							input.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToModuleOptions );
							i += ModuleOptionsRegionParser.LinesParsed;
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.TemplatesRegionToken )
						{
							++i;
							var dataTrimmedToTemplates = data[ i.. ];
							input.Templates = TemplatesRegionParser.Parse( dataTrimmedToTemplates );
							i += ModuleOptionsRegionParser.LinesParsed;
						}

						if ( tokenParser.IndentLevel > 0 )
						{
							throw new RegionNotClosedException( $"A region was not closed properly when parsing token \"{qualifiedToken.Value}\"" );
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
						throw new UnknownTokenException( $"An unknown token ( \"{qualifiedToken.Value}\" ) was read when a region name was expected" );
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
			catch ( Exception exception )
			{
				throw new FileNotFoundException( "Failed to read data from input file", exception );
			}
		}
	}
}
