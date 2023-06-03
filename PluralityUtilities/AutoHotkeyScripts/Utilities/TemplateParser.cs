using System.Text;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.AutoHotkeyScripts.LookUpTables;
using PluralityUtilities.Common.Enums;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class TemplateParser
	{
		private TokenParser TokenParser = new TokenParser();


		public string[] GenerateMacrosFromInput( Input input )
		{
			var macros = new List< string >();
			foreach ( var entry in input.Entries )
			{
				macros.AddRange( CreateAllEntryMacrosFromTemplates( input.Templates, entry ) );
			}
			return macros.ToArray();
		}

		public string[] ParseTemplatesFromData( string[] data, ref int i )
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
					case TokenQualifiers.OpenBracket:
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


		private List< string > CreateAllIdentityMacrosFromTemplates( string[] templates, Identity identity, string pronoun, string decoration )
		{
			var macros = new List< string >();
			foreach ( var template in templates )
			{
				macros.Add( CreateIdentityMacroFromTemplate( template, identity, pronoun, decoration ) );
			}
			return macros;
		}

		private List< string > CreateAllEntryMacrosFromTemplates( string[] templates, Entry entry )
		{
			var macros = new List< string >();
			foreach ( var identity in entry.Identities )
			{
				macros.AddRange( CreateAllIdentityMacrosFromTemplates( templates, identity, entry.Pronoun, entry.Decoration ) );
			}
			return macros;
		}

		private string CreateIdentityMacroFromTemplate( string template, Identity identity, string pronoun, string decoration )
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

		private string ParseTemplateFromInputLine( string input )
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
