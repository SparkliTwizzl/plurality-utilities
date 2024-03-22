using Petrichor.App.Syntax;
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
		public static void GenerateScript( ModuleCommand command )
		{
			Log.Important( "Generating text shortcuts script..." );

			var outputFilePath = command.Options[ CommandOptions.ShortcutScriptOptionOutputFile ];
			var data = command.Data;
			GenerateAutoHotkeyScript( data, outputFilePath );

			var successMessage = "Generated text shortcuts script successfully.";
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

		private static DataRegionParser<ScriptEntry> CreateEntryRegionParser()
		{
			var colorTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Color = token.Value;
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var decorationTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Decoration = token.Value;
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var idTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.ID = token.Value;
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var nameTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Identities.Add( ParseNameTokenValue( token ) );
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var lastNameTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.LastIdentity = ParseNameTokenValue( token );
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var pronounTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptEntry result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Pronoun = token.Value;
				return new ProcessedRegionData<ScriptEntry>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new DataRegionParserDescriptor<ScriptEntry>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.Entry,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Color, colorTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Decoration, decorationTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.ID, idTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Name, nameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.LastName, lastNameTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.Pronoun, pronounTokenHandler },
				},
			};

			return new DataRegionParser<ScriptEntry>( parserDescriptor );
		}

		private static DataRegionParser<List<ScriptEntry>> CreateEntryListRegionParser()
		{
			var entryRegionParser = CreateEntryRegionParser();

			var entryTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<ScriptEntry> result ) =>
			{
				entryRegionParser.Reset();
				var dataTrimmedToRegion = regionData[ tokenStartIndex.. ];
				var entry = entryRegionParser.Parse( dataTrimmedToRegion );
				result.Add( entry );
				return new ProcessedRegionData<List<ScriptEntry>>()
				{
					BodySize = entryRegionParser.LinesParsed - 1,
					Value = result,
				};
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<ScriptEntry>>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.EntryList,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Entry, entryTokenHandler },
				},
				PostParseHandler = ( List<ScriptEntry> entries ) =>
				{
					Log.Info( $"Parsed {entries.Count} \"{ShortcutScriptGeneration.Syntax.Tokens.Entry.Key}\" tokens." );
					return entries;
				},
			};

			return new DataRegionParser<List<ScriptEntry>>( parserDescriptor );
		}

		private static DataRegionParser<ScriptModuleOptions> CreateModuleOptionsRegionParser()
		{
			var defaultIconTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.DefaultIconFilePath = filePath;
				Log.Info( $"Stored default icon file path ({filePath})." );
				return new ProcessedRegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var reloadShortcutTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.ReloadShortcut = hotstring;
				Log.Info( $"Stored reload shortcut (\"{token.Value}\" -> \"{hotstring}\")." );
				return new ProcessedRegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var suspendIconTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var filePath = token.Value.WrapInQuotes();
				result.SuspendIconFilePath = filePath;
				Log.Info( $"Stored suspend icon file path ({filePath})." );
				return new ProcessedRegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var suspendShortcutTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
				result.SuspendShortcut = hotstring;
				Log.Info( $"Stored suspend shortcut (\"{token.Value}\" -> \"{hotstring}\")." );
				return new ProcessedRegionData<ScriptModuleOptions>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new DataRegionParserDescriptor<ScriptModuleOptions>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.ModuleOptions,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.DefaultIcon, defaultIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.ReloadShortcut, reloadShortcutTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.SuspendIcon, suspendIconTokenHandler },
					{ ShortcutScriptGeneration.Syntax.Tokens.SuspendShortcut, suspendShortcutTokenHandler },
				},
			};

			return new DataRegionParser<ScriptModuleOptions>( parserDescriptor );
		}

		private static DataRegionParser<List<string>> CreateTemplateListRegionParser()
		{
			var templateTokenHandler = ( IndexedString[] regionData, int tokenStartIndex, List<string> result ) =>
			{
				var token = new StringToken( regionData[ tokenStartIndex ] );
				result.Add( ParseTemplateFromLine( token.Value, token.LineNumber ) );
				return new ProcessedRegionData<List<string>>()
				{
					Value = result,
				};
			};

			var parserDescriptor = new DataRegionParserDescriptor<List<string>>()
			{
				RegionToken = ShortcutScriptGeneration.Syntax.Tokens.TemplateList,
				TokenHandlers = new()
				{
					{ ShortcutScriptGeneration.Syntax.Tokens.Template, templateTokenHandler },
				},
				PostParseHandler = ( List<string> templates ) =>
				{
					Log.Info( $"Parsed {templates.Count} \"{ShortcutScriptGeneration.Syntax.Tokens.Template.Key}\" tokens." );
					return templates;
				},
			};

			return new DataRegionParser<List<string>>( parserDescriptor );
		}

		private static string ExtractFindString( string input, int lineNumber )
		{
			var lengthOfFindString = GetLengthOfFindString( input, lineNumber );
			return input[ ..( lengthOfFindString + 1 ) ];
		}

		private static void GenerateAutoHotkeyScript( IndexedString[] data, string outputFilePath )
		{
			try
			{
				var input = new ShortcutScriptGeneration.Utilities.InputHandler(
					moduleOptionsRegionParser: CreateModuleOptionsRegionParser(),
					entryListRegionParser: CreateEntryListRegionParser(),
					templateListRegionParser: CreateTemplateListRegionParser(),
					macroGenerator: new MacroGenerator() )
						.ParseRegionData( data );
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
				ExceptionLogger.LogAndThrow( new TokenValueException( $"A {token.Key} token had an invalid value ( \"{token.Value}\" )." ), token.LineNumber );
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
