﻿using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ModuleOptionsRegionParser : IModuleOptionsRegionParser
	{
		private int IndentLevel { get; set; } = 0;


		public bool HasParsedMaxAllowedRegions { get; private set; } = false;
		public int LinesParsed { get; private set; } = 0;
		public int MaxRegionsAllowed { get; private set; } = 1;
		public int RegionsParsed { get; private set; } = 0;


		public ScriptModuleOptions Parse( string[] regionData )
		{
			var taskMessage = $"Parse region: {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName}";
			Log.TaskStart( taskMessage );

			if ( HasParsedMaxAllowedRegions )
			{
				throw new FileRegionException( $"Input file cannot contain more than {MaxRegionsAllowed} {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} regions" );
			}

			var moduleOptions = new ScriptModuleOptions();
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
						throw new BracketMismatchException( $"A mismatched closing bracket was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region" );
					}

					if ( IndentLevel == 0 )
					{
						isParsingFinished = true;
					}
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.DefaultIconFilePathTokenName )
				{
					moduleOptions.DefaultIconFilePath = token.Value.WrapInQuotes();
					Log.Info( $"Stored token {ShortcutScriptGenerationSyntax.DefaultIconFilePathTokenName}" );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.ReloadShortcutTokenName )
				{
					moduleOptions.ReloadShortcut = token.Value;
					Log.Info( $"Stored token {ShortcutScriptGenerationSyntax.ReloadShortcutTokenName}" );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendIconFilePathTokenName )
				{
					moduleOptions.SuspendIconFilePath = token.Value.WrapInQuotes();
					Log.Info( $"Stored token {ShortcutScriptGenerationSyntax.SuspendIconFilePathTokenName}" );
				}

				else if ( token.Name == ShortcutScriptGenerationSyntax.SuspendShortcutTokenName )
				{
					moduleOptions.SuspendShortcut = token.Value;
					Log.Info( $"Stored token {ShortcutScriptGenerationSyntax.SuspendShortcutTokenName}" );
				}

				else
				{
					throw new TokenException( $"An unrecognized token (\"{rawToken.Trim()}\") was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region" );
				}

				if ( isParsingFinished )
				{
					LinesParsed = i;
					break;
				}
			}

			if ( IndentLevel != 0 )
			{
				throw new BracketMismatchException( $"A mismatched curly brace was found when parsing {ShortcutScriptGenerationSyntax.ModuleOptionsRegionTokenName} region" );
			}

			++RegionsParsed;
			HasParsedMaxAllowedRegions = RegionsParsed >= MaxRegionsAllowed;

			Log.TaskFinish( taskMessage );
			return moduleOptions;
		}
	}
}
