using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.LookUpTables;
using Petrichor.ShortcutScriptGeneration.Syntax;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class TemplatesRegionParser : ITemplatesRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private static string RegionName => TokenNames.TemplatesRegion;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed => 1;
		public int RegionsParsed { get; private set; } = 0;
		public int TemplatesParsed { get; private set; } = 0;


		public string[] Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: { RegionName }";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				ExceptionLogger.LogAndThrow( new FileRegionException( $"Input file cannot contain more than { MaxRegionsAllowed } { RegionName } regions" ) );
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

				else if ( token.Name == Common.Syntax.TokenNames.RegionOpen )
				{
					++IndentLevel;
				}

				else if ( token.Name == Common.Syntax.TokenNames.RegionClose )
				{
					--IndentLevel;

					if ( IndentLevel < 0 )
					{
						ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched closing bracket was found when parsing region: { RegionName }" ) );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == TokenNames.Template )
				{
					templates.Add( ParseTemplateFromLine( token.Value ) );
					continue;
				}

				else
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"An unrecognized token (\"{ rawToken.Trim() }\") was found when parsing region: { RegionName }" ) );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i + 1;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				ExceptionLogger.LogAndThrow( new BracketException( $"A mismatched curly brace was found when parsing region: { RegionName }" ) );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			TemplatesParsed = templates.Count;
			Log.Info( $"Parsed { TemplatesParsed } templates" );
			Log.TaskFinish( taskMessage );
			return templates.ToArray();
		}


		private static string ConvertPetrichorTemplateToAHK( string line )
		{
			var components = line.Split( "::" );
			var findString = $"::{ components[ 0 ].Trim() }::";
			var replaceString = components.Length > 1 ? components[ 1 ].Trim() : "";
			return $"{ findString }{ replaceString }";
		}

		private static int GetIndexOfNextFindStringCloseChar( string input )
		{
			var nextCloseCharIndex = input.IndexOf( Common.Syntax.OperatorChars.TokenNameClose );
			if ( nextCloseCharIndex < 0 )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"A template contained a mismatched find-string open character ('{ Common.Syntax.OperatorChars.TokenNameOpen }')" ) );
			}

			var isCloseCharEscaped = input[ nextCloseCharIndex - 1 ] == '\\';
			if ( isCloseCharEscaped )
			{
				var substring = input[ ( nextCloseCharIndex + 1 ).. ];
				return nextCloseCharIndex + GetIndexOfNextFindStringCloseChar( substring );
			}

			return nextCloseCharIndex;
		}

		private static int GetLengthOfFindString( string input ) => GetIndexOfNextFindStringCloseChar( input );

		private static string ParseTemplateFromLine( string line )
		{
			var rawHotstring = ConvertPetrichorTemplateToAHK( line );
			var sanitizedHotstring = SanitizeHotstring( rawHotstring );

			var template = new StringBuilder();
			for ( var i = 0 ; i < sanitizedHotstring.Length ; ++i )
			{
				var c = sanitizedHotstring[ i ];
				if ( c == Common.Syntax.OperatorChars.TokenNameClose )
				{
					ExceptionLogger.LogAndThrow( new TokenException( $"A template contained a mismatched find-string close character ('{ Common.Syntax.OperatorChars.TokenNameClose}')" ) );
				}

				else if ( c == Common.Syntax.OperatorChars.TokenNameOpen )
				{
					var substring = sanitizedHotstring[ i.. ];
					var findString = ValidateAndExtractFindString( substring );
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
						ExceptionLogger.LogAndThrow( new EscapeCharacterException( $"A template contained a dangling escape character ('{ Common.Syntax.OperatorChars.Escape }') with no following character to escape", exception ) );
					}
				}

				else
				{
					_ = template.Append( c );
				}
			}
			return template.ToString();
		}

		private static string SanitizeHotstring( string rawHotstring )
			=> rawHotstring
				.Replace( $"{ Common.Syntax.OperatorChars.Escape}{ Common.Syntax.OperatorChars.Escape}", Common.Syntax.OperatorChars.EscapeStandin )
				.Replace( $"{ Common.Syntax.OperatorChars.Escape}{ Common.Syntax.OperatorChars.TokenNameOpen}", Common.Syntax.OperatorChars.TokenNameOpenStandin )
				.Replace( $"{ Common.Syntax.OperatorChars.Escape}{ Common.Syntax.OperatorChars.TokenNameClose}", Common.Syntax.OperatorChars.TokenNameCloseStandin );

		private static string ValidateAndExtractFindString( string input )
		{
			var lengthOfFindString = GetLengthOfFindString( input );
			var findString = input[ ..( lengthOfFindString + 1 ) ];
			if ( !ScriptTemplateFindStrings.LookUpTable.Contains( findString ) )
			{
				ExceptionLogger.LogAndThrow( new TokenException( $"A template contained an unknown find-string \"{ findString }\"" ) );
			}
			return findString;
		}
	}
}
