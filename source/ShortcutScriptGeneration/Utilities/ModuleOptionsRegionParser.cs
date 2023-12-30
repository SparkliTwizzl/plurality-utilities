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


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;


		public ScriptModuleOptions Parse( string[] regionData, ref int i )
		{
			var taskMessage = $"parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region data";
			Log.TaskStarted( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				var errorMessage = $"input file cannot contain more than {MaxRegionsAllowed} {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} regions";
				Log.Error( errorMessage );
				throw new FileRegionException( errorMessage );
			}

			var moduleOptions = new ScriptModuleOptions();
			for ( ; i < regionData.Length ; ++i )
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
					moduleOptions.DefaultIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ReloadShortcutTokenName )
				{
					moduleOptions.ReloadShortcut = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendIconFilePathTokenName )
				{
					moduleOptions.SuspendIconFilePath = token.Value;
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendShortcutTokenName )
				{
					moduleOptions.SuspendShortcut = token.Value;
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

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinished( taskMessage );
			return moduleOptions;
		}
	}
}
