﻿using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class InputFileHandler
	{
		private IEntriesRegionParser EntriesRegionParser { get; set; }
		private IRegionParser< ScriptInput > FileRegionParser { get; set; }
		private IMacroGenerator MacroGenerator { get; set; }
		private IRegionParser< StringWrapper > MetadataRegionParser { get; set; }
		private IModuleOptionsRegionParser ModuleOptionsRegionParser { get; set; }
		private ITemplatesRegionParser TemplatesRegionParser { get; set; }


		private Func< string[], int, ScriptInput, RegionData< ScriptInput > > EntriesTokenHandler =>
			( string[] fileData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
					result.Entries = EntriesRegionParser.Parse( dataTrimmedToRegion );
					return new RegionData< ScriptInput >()
					{
						BodySize = EntriesRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func< string[], int, ScriptInput, RegionData< ScriptInput > > MetadataTokenHandler =>
			( string[] fileData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
					var resultMessage = MetadataRegionParser.Parse( dataTrimmedToRegion ) ;
					Log.Important( resultMessage.ToString() );
					return new RegionData< ScriptInput >()
					{
						BodySize = MetadataRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func< string[], int, ScriptInput, RegionData< ScriptInput > > ModuleOptionsTokenHandler =>
			( string[] fileData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
					result.ModuleOptions = ModuleOptionsRegionParser.Parse( dataTrimmedToRegion );
					return new RegionData< ScriptInput >()
					{
						BodySize = ModuleOptionsRegionParser.LinesParsed,
						Value = result,
					};
				};

		private Func< ScriptInput, ScriptInput > PostParseHandler => ( ScriptInput result ) =>
		{
			if ( !MetadataRegionParser.HasParsedMinRequiredRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input files must contain at least { MetadataRegionParser.MinRegionsRequired } { MetadataRegionParser.RegionName } regions" ) );
			}

			result.Macros = MacroGenerator.Generate( result );
			return result;
		};

		private Func< string[], int, ScriptInput, RegionData< ScriptInput > > TemplatesTokenHandler =>
			( string[] fileData, int regionStartIndex, ScriptInput result ) =>
				{
					var dataTrimmedToRegion = fileData[ ( regionStartIndex + 1 ).. ];
					result.Templates = TemplatesRegionParser.Parse( dataTrimmedToRegion );
					return new RegionData< ScriptInput >()
					{
						BodySize = TemplatesRegionParser.LinesParsed,
						Value = result,
					};
				};


		public InputFileHandler( IRegionParser< StringWrapper > metadataRegionParser, IModuleOptionsRegionParser moduleOptionsRegionParser, IEntriesRegionParser entriesRegionParser, ITemplatesRegionParser templatesRegionParser, IMacroGenerator macroGenerator )
		{
			EntriesRegionParser = entriesRegionParser;
			MacroGenerator = macroGenerator;
			MetadataRegionParser = metadataRegionParser;
			ModuleOptionsRegionParser = moduleOptionsRegionParser;
			TemplatesRegionParser = templatesRegionParser;
			FileRegionParser = CreateRegionParser();
		}


		public ScriptInput ProcessFile( string filePath )
		{
			var taskMessage = $"Parse input file \"{ filePath }\"";
			Log.TaskStart( taskMessage );

			var fileData = File.ReadAllLines( filePath );
			var result = FileRegionParser.Parse( fileData );

			Log.TaskFinish( taskMessage );
			return result;
		}


		private RegionParser< ScriptInput > CreateRegionParser()
		{
			var tokenHandlers = new Dictionary< string, Func< string[], int, ScriptInput, RegionData< ScriptInput > > >()
			{
				{ Syntax.TokenNames.EntriesRegion, EntriesTokenHandler },
				{ Common.Syntax.TokenNames.MetadataRegion, MetadataTokenHandler },
				{ Syntax.TokenNames.ModuleOptionsRegion, ModuleOptionsTokenHandler },
				{ Syntax.TokenNames.TemplatesRegion, TemplatesTokenHandler },
			};

			var parserDescriptor = new RegionParserDescriptor< ScriptInput >()
			{
				RegionName = "Input file body",
				MaxRegionsAllowed = 1,
				MinRegionsRequired = 1,
				TokenHandlers = tokenHandlers,
				PostParseHandler = PostParseHandler,
				MaxAllowedTokenInstances = new()
				{
					{ Syntax.TokenNames.EntriesRegion, Info.TokenMetadata.MaxEntriesRegions },
					{ Common.Syntax.TokenNames.MetadataRegion, Common.Info.TokenMetadata.MaxMetadataRegions },
					{ Syntax.TokenNames.ModuleOptionsRegion, Info.TokenMetadata.MaxModuleOptionsRegions },
					{ Syntax.TokenNames.TemplatesRegion, Info.TokenMetadata.MaxTemplatesRegions },
				},
				MinRequiredTokenInstances = new()
				{
					{ Syntax.TokenNames.EntriesRegion, Info.TokenMetadata.MinEntriesRegions },
					{ Common.Syntax.TokenNames.MetadataRegion, Common.Info.TokenMetadata.MinMetadataRegions },
					{ Syntax.TokenNames.ModuleOptionsRegion, Info.TokenMetadata.MinModuleOptionsRegions },
					{ Syntax.TokenNames.TemplatesRegion, Info.TokenMetadata.MinTemplatesRegions },
				},
			};

			return new RegionParser< ScriptInput >( parserDescriptor );
		}
	}
}