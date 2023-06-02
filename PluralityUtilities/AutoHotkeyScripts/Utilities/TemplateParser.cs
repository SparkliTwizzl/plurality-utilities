using System.Text;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.AutoHotkeyScripts.LookUpTables;
using PluralityUtilities.Common.Enums;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class TemplateParser
	{
		private static TokenParser TokenParser = new TokenParser();


		public static string[] CreateMacrosFromInput( Entry[] entries, string[] templates )
		{
			var results = new List< string >();
			foreach ( var person in entries )
			{
				results.AddRange( CreateAllEntryMacrosFromTemplates( templates, person ) );
			}
			return results.ToArray();
		}

		public static string[] ParseTemplatesFromFile( string[] data, ref int i )
		{
			Log.WriteLineTimestamped( "started parsing templates from data" );
			var templates = new List< string >();
			var expectedTokens = new string[] { };
			for ( ; i < data.Length; ++i )
			{
				var token = TokenParser.ParseToken( data[ i ], expectedTokens);
				var isParsingFinished = false;
				switch (token.Qualifier)
				{
					case TokenQualifiers.BlankLine:
						break;
					case TokenQualifiers.CloseBracket:
						if ( TokenParser.IndentLevel < 1 )
						{
							isParsingFinished = true;
						}
						break;
					default:
						templates.Add( ParseTemplateFromInputLine( token.Value ) );
						break;
				}
				if ( isParsingFinished )
				{
					break;
				}
			}
			Log.WriteLineTimestamped( "finished parsing templates from data" );
			return templates.ToArray();
		}


		private static List< string > CreateAllIdentityMacrosFromTemplates( string[] templates, Identity identity, string pronoun, string decoration )
		{
			var results = new List< string >();
			foreach ( var template in templates )
			{
				results.Add( CreateIdentityMacroFromTemplate( template, identity, pronoun, decoration ) );
			}
			return results;
		}

		private static List< string > CreateAllEntryMacrosFromTemplates( string[] templates, Entry entry )
		{
			var results = new List< string >();
			foreach ( var identity in entry.Identities )
			{
				results.AddRange( CreateAllIdentityMacrosFromTemplates( templates, identity, entry.Pronoun, entry.Decoration ) );
			}
			return results;
		}

		private static string CreateIdentityMacroFromTemplate( string template, Identity identity, string pronoun, string decoration )
		{
			var macro = template;
			Dictionary< string, string > fields = new Dictionary< string, string >()
			{
				{ "name", identity.Name },
				{ "tag", identity.Tag },
				{ "pronoun", pronoun },
				{ "decoration", decoration },
			 };
			foreach ( var marker in TemplateMarkers.LookUpTable )
			{
				macro = macro.Replace( $"`{ marker.Value }`", fields[ marker.Value ] );
			}
			return macro;
		}

		private static string ParseTemplateFromInputLine( string input )
		{
			StringBuilder template = new StringBuilder();
			input = input.Trim();
			for ( int i = 0; i < input.Length; ++i )
			{
				var c = input[ i ];
				if ( c == '\\' )
				{
					try
					{
						template.Append( input[ i + 1 ] );
						++i;
						continue;
					}
					catch ( Exception ex )
					{
						var error = "a template contained a trailing escape character ('\\') with no following character to escape";
						Log.WriteLineTimestamped( $"error: { error }; { ex.Message }" );
						throw new EscapeCharacterMismatchException( error, ex );
					}
				}
				if ( TemplateMarkers.LookUpTable.TryGetValue( c, out var value ) )
				{
					template.Append( $"`{ value }`" );
				}
				else
				{
					template.Append( c );
				}
			}
			var result = template.ToString();
			Log.WriteLineTimestamped( $"parsed template: { result }" );
			return result;
		}
	}
}
