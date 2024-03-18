using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.ShortcutScriptGeneration.Utilities;
using Petrichor.ShortcutScriptGeneration.Syntax;
using Petrichor.ShortcutScriptGeneration.LookUpTables;


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
				var moduleOptionsRegionParser = CreateModuleOptionsRegionParser();

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
				Common.Info.AppVersion.RejectUnsupportedVersions( version );
				return new RegionData< StringWrapper >();
			};

			var parserDescriptor = new RegionParserDescriptor< StringWrapper >()
			{
				RegionName = Common.Syntax.TokenNames.MetadataRegion,
				TokenHandlers = new()
				{
					{ Common.Syntax.TokenNames.MinimumVersion, minimumVersionTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ Common.Syntax.TokenNames.MinimumVersion, Common.Info.TokenMetadata.MaxMinimumVersionTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ Common.Syntax.TokenNames.MinimumVersion, Common.Info.TokenMetadata.MinMinimumVersionTokens },
				},
			};

			return new RegionParser< StringWrapper >( parserDescriptor );
		}

		private static RegionParser< ScriptModuleOptions > CreateModuleOptionsRegionParser()
		{
			var defaultIconTokenHandler = ( string[] fileData, int regionStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.DefaultIconFilePath = filePath;
				Log.Info( $"Stored path to default icon ({ filePath })" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var reloadShortcutTokenHandler = ( string[] fileData, int regionStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.ReloadShortcut = hotstring;
				Log.Info( $"Stored reload shortcut ({ token.Value } => { hotstring })" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var suspendIconTokenHandler = ( string[] fileData, int regionStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.SuspendIconFilePath = filePath;
				Log.Info( $"Stored path to suspend icon ({ filePath })" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var suspendShortcutTokenHandler = ( string[] fileData, int regionStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ regionStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.SuspendShortcut = hotstring;
				Log.Info( $"Stored suspend shortcut ({ token.Value } => { hotstring })" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor< ScriptModuleOptions >()
			{
				RegionName = TokenNames.ModuleOptionsRegion,
				TokenHandlers = new()
				{
					{ TokenNames.DefaultIconFilePath, defaultIconTokenHandler },
					{ TokenNames.ReloadShortcut, reloadShortcutTokenHandler },
					{ TokenNames.SuspendIconFilePath, suspendIconTokenHandler },
					{ TokenNames.SuspendShortcut, suspendShortcutTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ TokenNames.DefaultIconFilePath, TokenMetadata.MaxDefaultIconTokens },
					{ TokenNames.ReloadShortcut, TokenMetadata.MaxReloadShortcutTokens },
					{ TokenNames.SuspendIconFilePath, TokenMetadata.MaxSuspendIconTokens },
					{ TokenNames.SuspendShortcut, TokenMetadata.MaxSuspendShortcutTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ TokenNames.DefaultIconFilePath, TokenMetadata.MinDefaultIconTokens },
					{ TokenNames.ReloadShortcut, TokenMetadata.MinReloadShortcutTokens },
					{ TokenNames.SuspendIconFilePath, TokenMetadata.MinSuspendIconTokens },
					{ TokenNames.SuspendShortcut, TokenMetadata.MinSuspendShortcutTokens },
				},
			};

			return new RegionParser< ScriptModuleOptions >( parserDescriptor );
		}

		private static string ReplaceFieldsInScriptControlHotstring( string hotstring )
		{
			foreach ( var keyValuePair in ScriptHotstringKeys.LookUpTable )
			{
				var find = keyValuePair.Key;
				var replace = keyValuePair.Value;
				hotstring = hotstring.Replace( find, replace );
			}

			return hotstring
				.Replace( $"{ Common.Syntax.OperatorChars.Escape }[", "[" )
				.Replace( $"{ Common.Syntax.OperatorChars.Escape }]", "]" );
		}
	}
}
