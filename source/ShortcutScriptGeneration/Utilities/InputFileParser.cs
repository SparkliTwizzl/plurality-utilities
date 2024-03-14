using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileParser
	{
		private IRegionParser< List< ScriptEntry > > EntriesRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser< StringWrapper > MetadataRegionParser { get; set; }
		private IRegionParser< ScriptModuleOptions > ModuleOptionsRegionParser { get; set; }
		private IRegionParser< List< string > > TemplatesRegionParser { get; set; }


		public InputFileParser( IRegionParser< StringWrapper > metadataRegionParser, IRegionParser< ScriptModuleOptions > moduleOptionsRegionParser, IRegionParser< List< ScriptEntry > > entriesRegionParser, IRegionParser< List< string > > templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;
		}


		public ScriptInput Parse( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
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

				else if ( token.Name == Common.Syntax.Tokens.RegionOpen )
				{
					ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched open bracket was found when parsing input file \"{ filePath }\"" ) );
				}

				else if ( token.Name == Common.Syntax.Tokens.RegionClose )
				{
					ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched close bracket was found when parsing input file \"{ filePath }\"" ) );
				}

				else if ( token.Name == TokenNames.EntriesRegion )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion ).ToArray();
					i += EntriesRegionParser.LinesParsed;
				}

				else if ( token.Name == TokenNames.ModuleOptionsRegion )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
					i += ModuleOptionsRegionParser.LinesParsed;
				}

				else if ( token.Name == Common.Syntax.TokenNames.MetadataRegion )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					_ = MetadataRegionParser.Parse( dataTrimmedToRegion );
					i += MetadataRegionParser.LinesParsed;
				}

				else if ( token.Name == TokenNames.TemplatesRegion )
				{
					++i;
					var dataTrimmedToRegion = data[ i.. ];
					input.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion ).ToArray();
					i += TemplatesRegionParser.LinesParsed;
				}

				else
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unknown token ( \"{ rawToken }\" ) was read when a region name was expected" ) );
				}

				if ( MetadataRegionParser.RegionsParsed == 0 )
				{
					ExceptionLogger.LogAndThrow( new FileRegionException( $"First region in input file must be a { Common.Syntax.TokenNames.MetadataRegion } region" ) );
				}
			}

			input.Macros = MacroGenerator.Generate( input );
			Log.TaskFinish( taskMessage );
			return input;
		}
	}
}
