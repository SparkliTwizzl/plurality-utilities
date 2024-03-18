using Petrichor.Common.Containers;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Common.Syntax;
using Petrichor.Logging;
using Petrichor.Logging.Enums;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static LogMode ActiveLogMode { get; set; } = LogMode.None;
		public static string InputFilePath { get; set; } = string.Empty;
		public static string OutputFilePath { get; set; } = string.Empty;


		public static void Execute() => CreateAutoHotkeyScript();

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}


		private static void CreateAutoHotkeyScript()
		{
			try
			{
				Log.Important( "Generating AutoHotkey shortcuts script..." );
				
				var metadataRegionParser = CreateMetadataRegionParser();

				var moduleOptionsRegionParser = new ModuleOptionsRegionParser();
				var entryRegionParser = new EntryRegionParser();
				var entriesRegionParser = new EntriesRegionParser( entryRegionParser );
				var templatesRegionParser = new TemplatesRegionParser();

				var macroGenerator = new MacroGenerator();

				var inputFileParser = new InputFileHandler( metadataRegionParser, moduleOptionsRegionParser, entriesRegionParser, templatesRegionParser, macroGenerator );
				var input = inputFileParser.ProcessFile( InputFilePath );
				var scriptGenerator = new ScriptGenerator( input );
				scriptGenerator.Generate( OutputFilePath );

				var successMessage = "Generated AutoHotkey shortcuts script successfully";
				if ( Log.IsLoggingToConsoleDisabled )
				{
					Console.WriteLine( successMessage );
				}
				Log.Important( successMessage );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ShortcutScriptGenerationException( $"Generating AutoHotkey shortcuts script failed: {exception.Message}", exception ) );
			}
		}

		private static RegionParser< StringWrapper > CreateMetadataRegionParser()
		{
			var minimumVersionTokenHandler = ( string[] fileData, int regionStartIndex, StringWrapper result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var version = token.Value;
				AppVersion.RejectUnsupportedVersions( version );
				return new RegionData< StringWrapper >();
			};

			var parserDescriptor = new RegionParserDescriptor< StringWrapper >()
			{
				RegionName = TokenNames.MetadataRegion,
				TokenHandlers = new()
				{
					{ TokenNames.MinimumVersion, minimumVersionTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ TokenNames.MinimumVersion, TokenMetadata.MaxMinimumVersionTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{TokenNames.MinimumVersion, TokenMetadata.MinMinimumVersionTokens },
				},
			};

			return new RegionParser< StringWrapper >( parserDescriptor );
		}
	}
}
