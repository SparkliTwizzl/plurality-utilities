using Petrichor.Common.Containers;
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
		private int IndentLevel { get; set; } = 0;


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

			var data = File.ReadAllLines( filePath );
			var input = new ScriptInput();

			for ( var i = 0 ; i < data.Length ; ++i )
			{
				var rawToken = data[ i ];
				var token = new StringToken( rawToken );

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == CommonSyntax.OpenBracketTokenName )
				{
					++IndentLevel;
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing input file \"{filePath}\"" );
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntriesRegionTokenName )
				{
					var dataTrimmedToEntries = data[ ( i + 1 ).. ];
					input.Entries = EntriesRegionParser.Parse( dataTrimmedToEntries );
					i += EntriesRegionParser.LinesParsed;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName )
				{
					var dataTrimmedToModuleOptions = data[ ( i + 1 ).. ];
					input.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToModuleOptions );
					i += ModuleOptionsRegionParser.LinesParsed;
				}

				else if ( token.Name == CommonSyntax.MetadataRegionTokenName )
				{
					var dataTrimmedToMetadata = data[ ( i + 1 ).. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToMetadata );
					i += MetadataRegionParser.LinesParsed;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.TemplatesRegionTokenName )
				{
					var dataTrimmedToTemplates = data[ ( i + 1 ).. ];
					input.Templates = TemplatesRegionParser.Parse( dataTrimmedToTemplates );
					i += ModuleOptionsRegionParser.LinesParsed;
				}

				else
				{
					throw new TokenException( $"An unknown token ( \"{rawToken}\" ) was read when a region name was expected" );
				}

				if ( IndentLevel > 0 )
				{
					throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing input file \"{filePath}\"" );
				}

				if ( MetadataRegionParser.RegionsParsed == 0 )
				{
					throw new FileRegionException( $"First region in input file must be a {CommonSyntax.MetadataRegionTokenName} region" );
				}
			}

			if ( IndentLevel != 0 )
			{
				throw new BracketMismatchException( $"A mismatched curly brace was found when parsing input file \"{filePath}\"" );
			}

			input.Macros = MacroGenerator.Generate( input );
			Log.TaskFinish( taskMessage );
			return input;
		}
	}
}
