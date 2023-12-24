using Petrichor.Common.Containers;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMetadataParser : IShortcutScriptMetadataParser
	{
		private const string DefaultIconFilePathToken = "default-icon";
		private const string ReloadShortcutToken = "reload-shortcut";
		private const string SuspendIconFilePathToken = "suspend-icon";

		private int IndentLevel { get; set; } = 0;
		private ShortcutScriptMetadata Metadata { get; set; } = new();


		public ShortcutScriptMetadata ParseMetadataFromData( string[] data, ref int i )
		{
			var taskMessage = "parsing metadata region data";
			Log.TaskStarted( taskMessage );

			for (; i < data.Length; ++i)
			{
				var token = new StringToken( data[ i ] );
				var isParsingFinished = false;
				string? errorMessage;
				switch (token.Name)
				{
					case "{":
						{
							++IndentLevel;
							break;
						}

					case "}":
						{
							--IndentLevel;

							if (IndentLevel < 0)
							{
								errorMessage = $"a mismatched closing curly brace was found when parsing metadata region";
								Log.Error(errorMessage);
								throw new BracketMismatchException(errorMessage);
							}

							if (IndentLevel == 0)
							{
								isParsingFinished = true;
							}
							break;
						}

					case DefaultIconFilePathToken:
						{
							Metadata.DefaultIconFilePath = token.Value;
							break;
						}

					case ReloadShortcutToken:
						{
							Metadata.ReloadShortcut = token.Value;
							break;
						}

					case SuspendIconFilePathToken:
						{
							Metadata.SuspendIconFilePath = token.Value;
							break;
						}

					default:
						{
							errorMessage = $"an unrecognized token ({token.Value}) was found when parsing metadata region";
							Log.Error(errorMessage);
							throw new UnknownTokenException(errorMessage);
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