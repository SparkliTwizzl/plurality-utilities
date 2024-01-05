using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class TemplatesRegionParser : ITemplatesRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => ShortcutScriptGenerationSyntax.TemplatesRegionTokenName;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed => 1;
		public int RegionsParsed { get; private set; } = 0;
		public int TemplatesParsed { get; private set; } = 0;


		public string[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {RegionName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {RegionName} regions" ) );
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
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found when parsing region: {RegionName}" ) );
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
					ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing region: {RegionName}" ) );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i + 1;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found when parsing region: {RegionName}" ) );
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
				if ( c == ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"A template contained a mismatched find-string close character ('{ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar}')" ) );
				}

				else if ( c == ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar )
				{
					var substring = line[ i.. ];
					var lengthOfFindString = GetLengthOfFindString( substring );
					_ = template.Append( line[ i..( i + lengthOfFindString + 1 ) ] );
					i += lengthOfFindString;
				}

				else if ( c == '\\' )
				{
					try
					{
						_ = template.Append( line[ i..( i + 2 ) ] );
						++i;
						continue;
					}
					catch ( Exception exception )
					{
						ExceptionLogger.LogAndThrow( new EscapeCharacterException( "A template contained a dangling escape character ('\\') with no following character to escape", exception ) );
					}
				}

				else
				{
					_ = template.Append( c );
				}
			}
			return template.ToString();
		}

		private static int GetLengthOfFindString( string input )
		{
			var nextCloseCharIndex = GetIndexOfNextFindStringCloseChar( input );
			return nextCloseCharIndex;
		}

		private static int GetIndexOfNextFindStringCloseChar( string input )
		{
			var nextCloseCharIndex = input.IndexOf( ShortcutScriptGenerationSyntax.TemplateFindStringCloseChar );
			if ( nextCloseCharIndex < 0 )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"A template contained a mismatched find-string open character ('{ShortcutScriptGenerationSyntax.TemplateFindStringOpenChar}')" ) );
			}

			var isCloseCharEscaped = input[ nextCloseCharIndex - 1 ] == '\\';
			if ( isCloseCharEscaped )
			{
				var substring = input[ ( nextCloseCharIndex + 1 ).. ];
				return nextCloseCharIndex + GetIndexOfNextFindStringCloseChar( substring );
			}

			return nextCloseCharIndex;
		}
	}
}
