using Petrichor.Common.Containers;
using Petrichor.Common.Syntax;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.LookUpTables;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public static class ModuleOptionsHandler
	{
		public static ProcessedRegionData<ScriptModuleOptions> DefaultIconTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			var filePath = token.Value.WrapInQuotes();
			result.DefaultIconFilePath = filePath;
			Log.Info( $"Stored default icon file path ({filePath})." );
			return new ProcessedRegionData<ScriptModuleOptions>( result );
		}

		public static ProcessedRegionData<ScriptModuleOptions> ReloadShortcutTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
			result.ReloadShortcut = hotstring;
			Log.Info( $"Stored reload shortcut (\"{token.Value}\" -> \"{hotstring}\")." );
			return new ProcessedRegionData<ScriptModuleOptions>( result );
		}

		public static ProcessedRegionData<ScriptModuleOptions> SuspendIconTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			var filePath = token.Value.WrapInQuotes();
			result.SuspendIconFilePath = filePath;
			Log.Info( $"Stored suspend icon file path ({filePath})." );
			return new ProcessedRegionData<ScriptModuleOptions>( result );
		}

		public static ProcessedRegionData<ScriptModuleOptions> SuspendShortcutTokenHandler( IndexedString[] regionData, int tokenStartIndex, ScriptModuleOptions result )
		{
			var token = new StringToken( regionData[ tokenStartIndex ] );
			var hotstring = ReplaceFieldsInScriptControlHotstring( token.Value );
			result.SuspendShortcut = hotstring;
			Log.Info( $"Stored suspend shortcut (\"{token.Value}\" -> \"{hotstring}\")." );
			return new ProcessedRegionData<ScriptModuleOptions>( result );
		}


		private static string ReplaceFieldsInScriptControlHotstring( string hotstring )
		{
			foreach ( var findTag in ScriptHotstringKeys.LookUpTable )
			{
				var find = findTag.Key;
				var replace = findTag.Value;
				hotstring = hotstring.Replace( find, replace );
			}

			return hotstring
				.Replace( $"{ControlSequences.Escape}{ControlSequences.FindTagOpen}", ControlSequences.FindTagOpen.ToString() )
				.Replace( $"{ControlSequences.Escape}{ControlSequences.FindTagClose}", ControlSequences.FindTagClose.ToString() );
		}
	}
}
