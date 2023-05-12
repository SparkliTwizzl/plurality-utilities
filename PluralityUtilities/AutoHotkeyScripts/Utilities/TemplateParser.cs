using System.Text;

using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.AutoHotkeyScripts.LookUpTables;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class TemplateParser
	{
		public static string[] CreateAllMacrosFromTemplates( Person[] people, string[] templates )
		{
			var results = new List< string >();
			foreach ( var person in people )
			{
				results.AddRange( CreateAllPersonMacrosFromTemplates( templates, person ) );
			}
			return results.ToArray();
		}

		public static string[] ParseTemplatesFromFile( string filePath )
		{
			Log.WriteLineTimestamped( $"parsing templates from file ({ filePath })..." );
			var inputData = File.ReadAllLines( filePath );
			var outputData = new List< string >();
			foreach ( var line in inputData )
			{
				outputData.Add( ParseTemplateFromInputLine( line ) );
			}
			return outputData.ToArray();
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

		private static List< string > CreateAllPersonMacrosFromTemplates( string[] templates, Person person )
		{
			var results = new List< string >();
			foreach ( var identity in person.Identities )
			{
				results.AddRange( CreateAllIdentityMacrosFromTemplates( templates, identity, person.Pronoun, person.Decoration ) );
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
