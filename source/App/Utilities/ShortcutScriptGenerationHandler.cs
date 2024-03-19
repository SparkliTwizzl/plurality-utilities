using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using Petrichor.ShortcutScriptGeneration.Utilities;
using System.Text;


namespace Petrichor.App.Utilities
{
	public static class ShortcutScriptGenerationHandler
	{
		public static void GenerateScript( string inputFilePath, string outputFilePath )
		{
			Log.Important( "Generating AutoHotkey shortcuts script..." );

			GenerateAutoHotkeyScript( inputFilePath, outputFilePath );

			var successMessage = "Generated AutoHotkey shortcuts script successfully";
			if ( Log.IsLoggingToConsoleDisabled )
			{
				Console.WriteLine( successMessage );
			}
			Log.Important( successMessage );
		}


		private static string ConvertTemplateToAutoHotkeySyntax( string line )
		{
			var components = line.Split( "::" );
			var findString = $"::{components[ 0 ].Trim()}::";
			var replaceString = components.Length > 1 ? components[ 1 ].Trim() : "";
			return $"{findString}{replaceString}";
		}

		private static RegionParser<List<ScriptEntry>> CreateEntriesRegionParser()
		{
			var entryRegionParser = CreateEntryRegionParser();

			var entryRegionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<ScriptEntry> result ) =>
			{
				entryRegionParser.Reset();
				var dataTrimmedToRegion = regionData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToRegion );
				result.Add( entry );
				return new RegionData<List<ScriptEntry>>()
				{
					BodySize = entryRegionParser.LinesParsed - 1,
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor<List<ScriptEntry>>()
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
				PostParseHandler = ( List<ScriptEntry> entries ) =>
				{
					Log.Info( $"Parsed {entries.Count} \"{ShortcutScriptGeneration.Syntax.TokenNames.EntryRegion}\" tokens" );
					return entries;
				},
			};

			return new RegionParser<List<ScriptEntry>>( parserDescriptor );
		}

		private static RegionParser<ScriptEntry> CreateEntryRegionParser()
		{
			var entryColorTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Color = token.Value;
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var entryDecorationTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Decoration = token.Value;
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var entryIDTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.ID = token.Value;
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var entryNameTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Identities.Add( ParseNameTokenValue( token ) );
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var entryLastNameTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.LastIdentity = ParseNameTokenValue( token );
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var entryPronounTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Pronoun = token.Value;
				return new RegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor<ScriptEntry>()
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

			return new RegionParser<ScriptEntry>( parserDescriptor );
		}

		private static RegionParser<IndexedString> CreateMetadataRegionParser()
		{
			var minimumVersionTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, IndexedString result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var version = token.Value;
				Common.Info.AppVersion.RejectUnsupportedVersions( version );
				return new RegionData<IndexedString>();
			};

			var parserDescriptor = new RegionParserDescriptor<IndexedString>()
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

			return new RegionParser<IndexedString>( parserDescriptor );
		}

		private static RegionParser<ScriptModuleOptions> CreateModuleOptionsRegionParser()
		{
			var defaultIconTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.DefaultIconFilePath = filePath;
				Log.Info( $"Stored default icon file path ( {filePath} )" );
				return new RegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var reloadShortcutTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.ReloadShortcut = hotstring;
				Log.Info( $"Stored reload shortcut ( \"{token.Value}\" -> \"{hotstring}\" )" );
				return new RegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var suspendIconTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.SuspendIconFilePath = filePath;
				Log.Info( $"Stored suspend icon file path ( {filePath} )" );
				return new RegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var suspendShortcutTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.SuspendShortcut = hotstring;
				Log.Info( $"Stored suspend shortcut ( \"{token.Value}\" -> \"{hotstring}\" )" );
				return new RegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor<ScriptModuleOptions>()
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

			return new RegionParser<ScriptModuleOptions>( parserDescriptor );
		}

		private static RegionParser<List<string>> CreateTemplatesRegionParser()
		{
			var templateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<string> result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Add( ParseTemplateFromLine( token.Value, token.LineNumber ) );
				return new RegionData<List<string>>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new RegionParserDescriptor<List<string>>()
			{
				RegionName = ShortcutScriptGeneration.Syntax.TokenNames.TemplatesRegion,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.Template, templateTokenHandler },
				},
				MaxAllowedTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.Template, ShortcutScriptGeneration.Info.TokenMetadata.MaxTemplateTokens },
				},
				MinRequiredTokenInstances = new()
				{
					{ ShortcutScriptGeneration.Syntax.TokenNames.Template, ShortcutScriptGeneration.Info.TokenMetadata.MinTemplateTokens },
				},
				PostParseHandler = ( List<string> templates ) =>
				{
					Log.Info( $"Parsed {templates.Count} \"{ShortcutScriptGeneration.Syntax.TokenNames.Template}\" tokens" );
					return templates;
				},
			};

			return new RegionParser<List<string>>( parserDescriptor );
		}

		private static string ExtractFindString( string input, int lineNumber )
		{
			var lengthOfFindString = GetLengthOfFindString( input, lineNumber );
			return input[ ..( lengthOfFindString + 1 ) ];
		}

		private static void GenerateAutoHotkeyScript( string inputFilePath, string outputFilePath )
		{
			try
			{
				var input = new InputFileHandler(
					CreateMetadataRegionParser(),
					CreateModuleOptionsRegionParser(),
					CreateEntriesRegionParser(),
					CreateTemplatesRegionParser(),
					new MacroGenerator() )
						.ProcessFile( inputFilePath );
				new ScriptGenerator().Generate( input, outputFilePath );
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ScriptGenerationException( $"Generating AutoHotkey shortcuts script failed.", exception ) );
			}
		}

		private static int GetIndexOfNextFindStringCloseChar( string input, int lineNumber )
		{
			var nextCloseCharIndex = input.IndexOf( Common.Syntax.OperatorChars.TokenNameClose );
			if ( nextCloseCharIndex < 0 )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched find-string open character ( '{Common.Syntax.OperatorChars.TokenNameOpen}' )." ), lineNumber );
			}

			var isCloseCharEscaped = input[ nextCloseCharIndex - 1 ] == '\\';
			if ( isCloseCharEscaped )
			{
				var substring = input[ ( nextCloseCharIndex + 1 ).. ];
				return nextCloseCharIndex + GetIndexOfNextFindStringCloseChar( substring, lineNumber );
			}

			return nextCloseCharIndex;
		}

		private static int GetLengthOfFindString( string input, int lineNumber ) => GetIndexOfNextFindStringCloseChar( input, lineNumber );

		private static ScriptIdentity ParseNameTokenValue( StringToken token )
		{
			var components = token.Value.Split( '@' );
			if ( components.Length != 2 )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A {token.Name} token had an invalid value ( \"{token.Value}\" )." ), token.LineNumber );
			}

			var name = components[ 0 ].Trim();
			var tag = components[ 1 ].Trim();
			var identity = new ScriptIdentity( name, tag );
			return identity;
		}

		private static string ParseTemplateFromLine( string line, int lineNumber )
		{
			var rawHotstring = ConvertTemplateToAutoHotkeySyntax( line );
			var sanitizedHotstring = SanitizeHotstring( rawHotstring );

			var template = new StringBuilder();
			for ( var i = 0 ; i < sanitizedHotstring.Length ; ++i )
			{
				var c = sanitizedHotstring[ i ];
				if ( c == Common.Syntax.OperatorChars.TokenNameClose )
				{
					ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained a mismatched find-string close character ('{Common.Syntax.OperatorChars.TokenNameClose}')." ), lineNumber );
				}

				else if ( c == Common.Syntax.OperatorChars.TokenNameOpen )
				{
					var substring = sanitizedHotstring[ i.. ];
					var findString = ExtractFindString( substring, lineNumber );
					ValidateFindString( findString, lineNumber );
					_ = template.Append( findString );
					var charsToSkip = findString.Length - 1;
					i += charsToSkip;
				}

				else if ( c == Common.Syntax.OperatorChars.Escape )
				{
					try
					{
						_ = template.Append( sanitizedHotstring[ i..( i + 2 ) ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						ExceptionLogger.LogAndThrow( new EscapeCharacterException( $"A template contained a dangling escape character ('{Common.Syntax.OperatorChars.Escape}') with no following character to escape.", exception ), lineNumber );
					}
				}

				else
				{
					_ = template.Append( c );
				}
			}
			return template.ToString();
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
				.Replace( $"{Common.Syntax.OperatorChars.Escape}[", "[" )
				.Replace( $"{Common.Syntax.OperatorChars.Escape}]", "]" );
		}

		private static string SanitizeHotstring( string rawHotstring )
			=> rawHotstring
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.Escape}", Common.Syntax.OperatorChars.EscapeStandin )
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.TokenNameOpen}", Common.Syntax.OperatorChars.TokenNameOpenStandin )
				.Replace( $"{Common.Syntax.OperatorChars.Escape}{Common.Syntax.OperatorChars.TokenNameClose}", Common.Syntax.OperatorChars.TokenNameCloseStandin );

		private static void ValidateFindString( string findString, int lineNumber )
		{
			if ( !ScriptTemplateFindStrings.LookUpTable.Contains( findString ) )
			{
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A template contained an unknown \"find\" string ( \"{findString}\" )." ), lineNumber );
			}
		}
	}
}
