using Petrichor.Common.Containers;
using Petrichor.Common.Info;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ModuleOptionsRegionParser : IModuleOptionsRegionParser
	{
		private int IndentLevel { get; set; } = 0;
		private ShortcutScriptModuleOptions ModuleOptions { get; set; } = new();


		public ShortcutScriptModuleOptions ParseModuleOptionsFromData( string[] data, ref int i )
		{
			var taskMessage = $"parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region data";
			Log.TaskStarted( taskMessage );

			for ( ; i < data.Length ; ++i )
			{
				var rawToken = data[ i ];
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
						var errorMessage = $"a mismatched closing bracket was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region";
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
					ModuleOptions.DefaultIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ReloadShortcutTokenName )
				{
					ModuleOptions.ReloadShortcut = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendIconFilePathTokenName )
				{
					ModuleOptions.SuspendIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendShortcutTokenName )
				{
					ModuleOptions.SuspendShortcut = token.Value;
				}

				else
				{
					var errorMessage = $"an unrecognized token (\"{rawToken.Trim()}\") was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region";
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
				var errorMessage = $"a mismatched curly brace was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region";
				Log.Error( errorMessage );
				throw new BracketMismatchException( errorMessage );
			}

			Log.TaskFinished( taskMessage );
			return ModuleOptions;
		}
	}
}
