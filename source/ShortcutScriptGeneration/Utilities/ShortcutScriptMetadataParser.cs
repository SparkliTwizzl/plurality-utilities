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
				string? errorMessage;
				switch ( token.Name )
				{
					case "":
					case CommonSyntax.LineCommentToken:
					{
						break;
					}

					case CommonSyntax.OpenBracketToken:
					{
						++IndentLevel;
						break;
					}

					case CommonSyntax.CloseBracketToken:
					{
						--IndentLevel;

						if ( IndentLevel < 0 )
						{
							errorMessage = $"a mismatched closing bracket was found when parsing metadata region";
							Log.Error( errorMessage );
							throw new BracketMismatchException( errorMessage );
						}

						if ( IndentLevel == 0 )
						{
							isParsingFinished = true;
						}
						break;
					}

					case ShortcutScriptGenerationSyntax.DefaultIconFilePathToken:
					{
						Metadata.DefaultIconFilePath = token.Value;
						break;
					}

					case ShortcutScriptGenerationSyntax.ReloadShortcutToken:
					{
						Metadata.ReloadShortcut = token.Value;
						break;
					}

					case ShortcutScriptGenerationSyntax.SuspendIconFilePathToken:
					{
						Metadata.SuspendIconFilePath = token.Value;
						break;
					}

					case ShortcutScriptGenerationSyntax.SuspendShortcutToken:
					{
						Metadata.SuspendShortcut = token.Value;
						break;
					}

					default:
					{
						errorMessage = $"an unrecognized token (\"{rawToken.Trim()}\") was found when parsing metadata region";
						Log.Error( errorMessage );
						throw new UnknownTokenException( errorMessage );
					}
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
