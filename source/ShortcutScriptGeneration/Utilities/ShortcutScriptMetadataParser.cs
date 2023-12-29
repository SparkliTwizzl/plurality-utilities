using Petrichor.Common.Containers;
using Petrichor.Common.Info;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMetadataParser : IShortcutScriptMetadataParser
	{
		private int IndentLevel { get; set; } = 0;
		private ShortcutScriptMetadata Metadata { get; set; } = new();


		public ShortcutScriptMetadata ParseMetadataFromData( string[] data, ref int i )
		{
			var taskMessage = "parsing metadata region data";
			Log.TaskStarted( taskMessage );

			for ( ; i < data.Length ; ++i )
			{
				var rawToken = data[ i ];
				var token = new StringToken( rawToken );
				var isParsingFinished = false;

				if ( token.Name == string.Empty || token.Name == CommonSyntax.LineCommentTokenName )
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
						var errorMessage = $"a mismatched closing bracket was found when parsing metadata region";
						Log.Error( errorMessage );
						throw new BracketMismatchException( errorMessage );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.DefaultIconFilePathTokenName )
				{
					Metadata.DefaultIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ReloadShortcutTokenName )
				{
					Metadata.ReloadShortcut = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendIconFilePathTokenName )
				{
					Metadata.SuspendIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendShortcutTokenName )
				{
					Metadata.SuspendShortcut = token.Value;
				}

				else
				{
					var errorMessage = $"an unrecognized token (\"{rawToken.Trim()}\") was found when parsing metadata region";
					Log.Error( errorMessage );
					throw new UnknownTokenException( errorMessage );
				}

				if ( isParsingFinished )
				{
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				var errorMessage = $"a mismatched curly brace was found when parsing metadata region";
				Log.Error( errorMessage );
				throw new BracketMismatchException( errorMessage );
			}

			Log.TaskFinished( taskMessage );
			return Metadata;
		}
	}
}
