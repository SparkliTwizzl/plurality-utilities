using Petrichor.Common.Containers;
using Petrichor.Common.Enums;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptMetadataParser : IShortcutScriptMetadataParser
	{
		private const string DefaultIconFilePathToken = "default-icon:";

		private int IndentLevel { get; set; } = 0;
		private ShortcutScriptMetadata Metadata { get; set; } = new();


		public ShortcutScriptMetadata ParseMetadataFromData( string[] data, ref int i )
		{
			var taskMessage = "parsing metadata region data";
			Log.TaskStarted( taskMessage );

			var expectedTokens = new string[]
			{
				DefaultIconFilePathToken,
			};
			for (; i < data.Length; ++i)
			{
				var token = new StringToken( data[ i ] );
				var errorMessage = string.Empty;
				var isParsingFinished = false;
				switch ( token.Name )
				{
					case "{":
						{
							++IndentLevel;
							break;
						}

					case "}":
						{
							--IndentLevel;

							if ( IndentLevel < 0 )
							{
								errorMessage = $"a mismatched closing curly brace was found when parsing metadata region";
								Log.Error( errorMessage );
								throw new BracketMismatchException( errorMessage );
							}

							if ( IndentLevel == 0 )
							{
								isParsingFinished = true;
							}
							break;
						}

					case DefaultIconFilePathToken:
						{
							StoreDefaultIconPath( token.Value );
							break;
						}

					default:
						{
							errorMessage = $"an unrecognized token ({ token.Value }) was found when parsing metadata region";
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


		private void StoreDefaultIconPath( string filePath )
		{
			Metadata.DefaultIconPath = filePath;
		}
	}
}