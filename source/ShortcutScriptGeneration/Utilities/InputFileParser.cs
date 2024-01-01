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
					ExceptionLogger.LogAndThrow( new BracketMismatchException( $"A mismatched open bracket was found when parsing input file \"{filePath}\"" ) );
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					ExceptionLogger.LogAndThrow( new BracketMismatchException( $"A mismatched close bracket was found when parsing input file \"{filePath}\"" ) );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.EntriesRegionTokenName )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion );
					i += EntriesRegionParser.LinesParsed;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
					i += ModuleOptionsRegionParser.LinesParsed;
				}

				else if ( token.Name == CommonSyntax.MetadataRegionTokenName )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					i += MetadataRegionParser.LinesParsed;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.TemplatesRegionTokenName )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion );
					i += ModuleOptionsRegionParser.LinesParsed;
				}

				else
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unknown token ( \"{rawToken}\" ) was read when a region name was expected" ) );
				}

				if ( MetadataRegionParser.RegionsParsed == 0 )
				{
					ExceptionLogger.LogAndThrow( new FileRegionException( $"First region in input file must be a {CommonSyntax.MetadataRegionTokenName} region" ) );
				}
			}

			input.Macros = MacroGenerator.Generate( input );
			Log.TaskFinish( taskMessage );
			return input;
		}
	}
}
