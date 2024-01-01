using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class TemplatesRegionParser : ITemplatesRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => ShortcutScriptGenerationSyntax.TemplatesRegionTokenName;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;
		public int TemplatesParsed { get; private set; } = 0;


		public string[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {RegionName} regions" );
			}

			var templates = new List<string>();
			for ( var i = 0 ; i < regionData.Length ; ++i )
			{
				var rawToken = regionData[ i ];
				var token = new StringToken( rawToken );
				var isParsingFinished = false;

				if ( token.Name == string.Empty )
				{
					continue;
				}

				else if ( token.Name == CommonSyntax.OpenBracketTokenName )
				{
					++IndentLevel;
				}

				else if ( token.Name == CommonSyntax.CloseBracketTokenName )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing region: {RegionName}" );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.TemplateTokenName )
				{
					templates.Add( ParseTemplateFromLine( token.Value ) );
					continue;
				}

				else
				{
					throw new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing region: {RegionName}" );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				throw new BracketMismatchException( $"A mismatched curly brace was found when parsing region: {RegionName}" );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			TemplatesParsed = templates.Count;
			Log.Info( $"Parsed {TemplatesParsed} templates" );
			Log.TaskFinish( taskMessage );
			return templates.ToArray();
		}


		private static string ParseTemplateFromLine( string line )
		{
			var template = new StringBuilder();
			line = line.Trim();
			for ( var i = 0 ; i < line.Length ; ++i )
			{
				var c = line[ i ];
				if ( c == '\\' )
				{
					try
					{
						_ = template.Append( line[ i + 1 ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						throw new EscapeCharacterMismatchException( "A template contained a trailing escape character ('\\') with no following character to escape", exception );
					}
				}
				_ = ShortcutScriptTemplateMarkers.LookUpTable.TryGetValue( c, out var value )
					? template.Append( $"`{value}`" )
					: template.Append( c );
			}
			return template.ToString();
		}
	}
}
