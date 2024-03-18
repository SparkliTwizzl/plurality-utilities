using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using Petrichor.ShortcutScriptGeneration.Utilities;


namespace Petrichor.App.Utilities
{
	public class ShortcutScriptGenerationHandler
	{
		private string InputFilePath { get; set; } = string.Empty;
		private string OutputFilePath { get; set; } = string.Empty;


		public ShortcutScriptGenerationHandler() { }
		public ShortcutScriptGenerationHandler( string inputFilePath, string outputFilePath )
		{
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
		}


		public void GenerateScript() => GenerateAutoHotkeyScript();


		private static RegionParser< List< ScriptEntry > > CreateEntriesRegionParser()
		{
			var entryRegionTokenHandler = ( string[] fileData, int tokenStartIndex, List< ScriptEntry > result ) =>
			{
				var entryRegionParser = CreateEntryRegionParser(); // recreating this every time is stupid but trying to reuse it leaves garbage data in it
				var dataTrimmedToRegion = fileData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToRegion );
				result.Add( entry );
				return new RegionData< List< ScriptEntry > >()
				{
					BodySize = entryRegionParser.LinesParsed,
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor< List< ScriptEntry > >()
			{
				RegionName = ShortcutScriptGeneration.Syntax.TokenNames.EntriesRegion,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion, entryRegionTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryRegions },
				},
				MinRequiredTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryRegions },
				},
				PostParseHandler = ( List< ScriptEntry > result ) =>
				{
					Log.Info( $"Parsed { result.Count } { ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion } tokens" );
					return result;
				},
			};

			return new RegionParser< List< ScriptEntry > >( parserDescriptor );
		}

		private static RegionParser< ScriptEntry > CreateEntryRegionParser()
		{
			var entryColorTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.Color = token.Value;
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var entryDecorationTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.Decoration = token.Value;
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var entryIDTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.ID = token.Value;
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var entryNameTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.Identities.Add( ParseNameTokenValue( token ) );
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var entryLastNameTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.LastIdentity = ParseNameTokenValue( token );
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var entryPronounTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				result.Pronoun = token.Value;
				return new RegionData< ScriptEntry >()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor< ScriptEntry >()
			{
				RegionName = ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryColor, entryColorTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryDecoration, entryDecorationTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryID, entryIDTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryName, entryNameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryLastName, entryLastNameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryPronoun, entryPronounTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryColor, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryColorTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryDecoration, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryDecorationTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryID, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryIDTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryName, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryNameTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryLastName, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryLastNameTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryPronoun, ShortcutScriptGeneration.Info.TokenMetadata.MaxEntryPronounTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryColor, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryColorTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryDecoration, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryDecorationTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryID, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryIDTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryName, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryNameTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryLastName, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryLastNameTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.EntryPronoun, ShortcutScriptGeneration.Info.TokenMetadata.MinEntryPronounTokens },
				},
			};

			return new RegionParser< ScriptEntry >( parserDescriptor );
		}

		private static RegionParser< StringWrapper > CreateMetadataRegionParser()
		{
			var minimumVersionTokenHandler = ( string[] fileData, int tokenStartIndex, StringWrapper result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
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
			var defaultIconTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.DefaultIconFilePath = filePath;
				Log.Info( $"Stored path to default icon ( { filePath } )" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var reloadShortcutTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.ReloadShortcut = hotstring;
				Log.Info( $"Stored reload shortcut ( \"{ token.Value }\" -> \"{ hotstring }\" )" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var suspendIconTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.SuspendIconFilePath = filePath;
				Log.Info( $"Stored path to suspend icon ( { filePath } )" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};
			
			var suspendShortcutTokenHandler = ( string[] fileData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( fileData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.SuspendShortcut = hotstring;
				Log.Info( $"Stored suspend shortcut ( \"{ token.Value }\" -> \"{ hotstring }\" )" );
				return new RegionData< ScriptModuleOptions >()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor< ScriptModuleOptions >()
			{
				RegionName = ShortcutScriptGeneration.Syntax.TokenNames.ModuleOptionsRegion,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.DefaultIconFilePath, defaultIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.ReloadShortcut, reloadShortcutTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendIconFilePath, suspendIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendShortcut, suspendShortcutTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.DefaultIconFilePath, ShortcutScriptGeneration.Info.TokenMetadata.MaxDefaultIconTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.ReloadShortcut, ShortcutScriptGeneration.Info.TokenMetadata.MaxReloadShortcutTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendIconFilePath, ShortcutScriptGeneration.Info.TokenMetadata.MaxSuspendIconTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendShortcut, ShortcutScriptGeneration.Info.TokenMetadata.MaxSuspendShortcutTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.DefaultIconFilePath, ShortcutScriptGeneration.Info.TokenMetadata.MinDefaultIconTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.ReloadShortcut, ShortcutScriptGeneration.Info.TokenMetadata.MinReloadShortcutTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendIconFilePath, ShortcutScriptGeneration.Info.TokenMetadata.MinSuspendIconTokens },
					{ ShortcutScriptGeneration.Syntax.TokenNames.SuspendShortcut, ShortcutScriptGeneration.Info.TokenMetadata.MinSuspendShortcutTokens },
				},
			};

			return new RegionParser< ScriptModuleOptions >( parserDescriptor );
		}

		private void GenerateAutoHotkeyScript()
		{
			try
			{
				Log.Important( "Generating AutoHotkey shortcuts script..." );
				
				var metadataRegionParser = CreateMetadataRegionParser();
				var moduleOptionsRegionParser = CreateModuleOptionsRegionParser();
				var entriesRegionParser = CreateEntriesRegionParser();

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

		private static ScriptIdentity ParseNameTokenValue( StringToken token )
		{
			var components = token.Value.Split( '@' );
			if ( components.Length != 2 )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"A { token.Name } token had an invalid value ( \"{ token.Value }\" )" ) );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new ScriptIdentity( name, tag );
			return identity;
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
