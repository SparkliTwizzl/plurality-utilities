using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Syntax;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Provides methods to handle module options for text shortcut script generation.
	/// </summary>
	public static class ModuleOptionsHandler
	{
		/// <summary>
		/// The handler function for <see cref="TokenPrototypes.DefaultIcon"/> tokens.
		/// </summary>
		/// <param name="bodyData">The indexed string data representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token with in the body data.</param>
		/// <param name="result">The result to modify.</param>
		/// <returns>The modified <see cref="ModuleOptions"/> instance.</returns>
		public static ProcessedTokenData<ModuleOptions> DefaultIconTokenHandler( IndexedString[] bodyData, int tokenStartIndex, ModuleOptions result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			var filePath = token.TokenValue.WrapInQuotes();
			result.DefaultIconFilePath = filePath;
			Logger.Info( $"Stored default icon file path ({filePath})." );
			return new ProcessedTokenData<ModuleOptions>( result );
		}

		/// <summary>
		/// The handler function for <see cref="TokenPrototypes.ReloadShortcut"/> tokens.
		/// </summary>
		/// <param name="bodyData">The indexed string data representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token with in the body data.</param>
		/// <param name="result">The result to modify.</param>
		/// <returns>The modified <see cref="ModuleOptions"/> instance.</returns>
		public static ProcessedTokenData<ModuleOptions> ReloadShortcutTokenHandler( IndexedString[] bodyData, int tokenStartIndex, ModuleOptions result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			var hotstring = ReplaceFieldsInScriptControlHotstring( token.TokenValue );
			result.ReloadShortcut = hotstring;
			Logger.Info( $"Stored reload shortcut (\"{token.TokenValue}\" -> \"{hotstring}\")." );
			return new ProcessedTokenData<ModuleOptions>( result );
		}

		/// <summary>
		/// The handler function for <see cref="TokenPrototypes.SuspendIcon"/> tokens.
		/// </summary>
		/// <param name="bodyData">The indexed string data representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token with in the body data.</param>
		/// <param name="result">The result to modify.</param>
		/// <returns>The modified <see cref="ModuleOptions"/> instance.</returns>
		public static ProcessedTokenData<ModuleOptions> SuspendIconTokenHandler( IndexedString[] bodyData, int tokenStartIndex, ModuleOptions result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			var filePath = token.TokenValue.WrapInQuotes();
			result.SuspendIconFilePath = filePath;
			Logger.Info( $"Stored suspend icon file path ({filePath})." );
			return new ProcessedTokenData<ModuleOptions>( result );
		}

		/// <summary>
		/// The handler function for <see cref="TokenPrototypes.SuspendShortcut"/> tokens.
		/// </summary>
		/// <param name="bodyData">The indexed string data representing the body data.</param>
		/// <param name="tokenStartIndex">The start index of the token with in the body data.</param>
		/// <param name="result">The result to modify.</param>
		/// <returns>The modified <see cref="ModuleOptions"/> instance.</returns>
		public static ProcessedTokenData<ModuleOptions> SuspendShortcutTokenHandler( IndexedString[] bodyData, int tokenStartIndex, ModuleOptions result )
		{
			var token = new StringToken( bodyData[ tokenStartIndex ] );
			var hotstring = ReplaceFieldsInScriptControlHotstring( token.TokenValue );
			result.SuspendShortcut = hotstring;
			Logger.Info( $"Stored suspend shortcut (\"{token.TokenValue}\" -> \"{hotstring}\")." );
			return new ProcessedTokenData<ModuleOptions>( result );
		}


		private static string ReplaceFieldsInScriptControlHotstring( string hotstring )
		{
			foreach ( var findTag in FindAndReplaceMaps.ScriptControlKeyboardShortcuts )
			{
				var find = findTag.Key;
				var replace = findTag.Value;
				hotstring = hotstring.Replace( find, replace );
			}

			return hotstring
				.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagOpen}", Common.Syntax.ControlSequences.FindTagOpen.ToString() )
				.Replace( $"{Common.Syntax.ControlSequences.Escape}{Common.Syntax.ControlSequences.FindTagClose}", Common.Syntax.ControlSequences.FindTagClose.ToString() );
		}
	}
}
