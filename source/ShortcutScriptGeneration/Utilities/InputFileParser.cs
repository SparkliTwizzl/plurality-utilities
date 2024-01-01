using Petrichor.Common.Enums;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IMetadataRegionParser MetadataRegionParser { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }


		public InputFileParser( IMetadataRegionParser metadataRegionParser, IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
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
				CommonSyntax.MetadataRegionToken,
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
							var dataTrimmedToEntries = data[ ( i + 1 ).. ];
							input.Entries = EntriesRegionParser.Parse( dataTrimmedToEntries );
							i += EntriesRegionParser.LinesParsed;
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.ModuleOptionsRegionToken )
						{
							var dataTrimmedToModuleOptions = data[ ( i + 1 ).. ];
							input.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToModuleOptions );
							i += ModuleOptionsRegionParser.LinesParsed;
						}

						else if ( qualifiedToken.Value == CommonSyntax.MetadataRegionToken )
						{
							var dataTrimmedToMetadata = data[ ( i + 1 ).. ];
							_ = MetadataRegionParser.Parse( dataTrimmedToMetadata );
							i += MetadataRegionParser.LinesParsed;
						}

						else if ( qualifiedToken.Value == ShortcutScriptGenerationSyntax.TemplatesRegionToken )
						{
							var dataTrimmedToTemplates = data[ ( i + 1 ).. ];
							input.Templates = TemplatesRegionParser.Parse( dataTrimmedToTemplates );
							i += ModuleOptionsRegionParser.LinesParsed;
						}

						if ( tokenParser.IndentLevel > 0 )
						{
							throw new FileRegionException( $"A region was not closed properly when parsing token \"{qualifiedToken.Value}\"" );
						}

						if ( MetadataRegionParser.RegionsParsed == 0 )
						{
							throw new FileRegionException( $"First region in input file must be a {CommonSyntax.MetadataRegionTokenName} region" );
						}

						break;
					}

					case StringTokenQualifiers.Unknown:
					{
						throw new TokenException( $"An unknown token ( \"{qualifiedToken.Value}\" ) was read when a region name was expected" );
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
