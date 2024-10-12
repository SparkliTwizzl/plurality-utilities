using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;

namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Handles the parsing and processing of shortcut script input data.
	/// </summary>
	public class InputHandler
	{
		private ITokenBodyParser<List<Entry>> EntryListParser { get; set; }
		private ITokenBodyParser<ShortcutScriptInput> InputDataParser { get; set; }
		private ITokenBodyParser<ModuleOptions> ModuleOptionsParser { get; set; }
		private IShortcutProcessor ShortcutProcessor { get; set; }
		private ITokenBodyParser<ShortcutScriptInput> ShortcutListParser { get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="InputHandler"/> class.
		/// </summary>
		/// <param name="moduleOptionsParser">The parser for module options.</param>
		/// <param name="entryListParser">The parser for the entry list.</param>
		/// <param name="shortcutListParser">The parser for the shortcut list.</param>
		/// <param name="shortcutProcessor">The processor for shortcuts.</param>
		public InputHandler(ITokenBodyParser<ModuleOptions> moduleOptionsParser, ITokenBodyParser<List<Entry>> entryListParser, ITokenBodyParser<ShortcutScriptInput> shortcutListParser, IShortcutProcessor shortcutProcessor)
		{
			EntryListParser = entryListParser;
			ModuleOptionsParser = moduleOptionsParser;
			ShortcutProcessor = shortcutProcessor;
			ShortcutListParser = shortcutListParser;
			InputDataParser = CreateInputDataParser();
		}


		/// <summary>
		/// Parses the shortcut script input data.
		/// </summary>
		/// <param name="bodyData">The array of indexed strings representing the body data.</param>
		/// <returns>A <see cref="ShortcutScriptInput"/> object containing the parsed data.</returns>
		public ShortcutScriptInput ParseInputData(IndexedString[] bodyData) => InputDataParser.Parse(bodyData);


		/// <summary>
		/// Creates a parser for the shortcut script input.
		/// </summary>
		/// <returns>A <see cref="TokenBodyParser{ShortcutScriptInput}"/> object configured for parsing shortcut script input.</returns>
		private TokenBodyParser<ShortcutScriptInput> CreateInputDataParser()
		{
			var entryListTokenHandler = (IndexedString[] bodyData, int regionStartIndex, ShortcutScriptInput result) =>
			{
				var dataTrimmedToToken = bodyData[regionStartIndex..];
				result.Entries = EntryListParser.Parse(dataTrimmedToToken).ToArray();
				return new ProcessedTokenData<ShortcutScriptInput>(value: result, bodySize: EntryListParser.TotalLinesParsed);
			};

			var moduleOptionsTokenHandler = (IndexedString[] bodyData, int regionStartIndex, ShortcutScriptInput result) =>
			{
				var dataTrimmedToToken = bodyData[regionStartIndex..];
				result.ModuleOptions = ModuleOptionsParser.Parse(dataTrimmedToToken);
				return new ProcessedTokenData<ShortcutScriptInput>(value: result, bodySize: ModuleOptionsParser.TotalLinesParsed);
			};

			var shortcutListTokenHandler = (IndexedString[] bodyData, int regionStartIndex, ShortcutScriptInput result) =>
			{
				var dataTrimmedToToken = bodyData[regionStartIndex..];
				result = ShortcutListParser.Parse(dataTrimmedToToken, result);
				return new ProcessedTokenData<ShortcutScriptInput>(value: result, bodySize: ShortcutListParser.TotalLinesParsed);
			};

			var tokenParseDescriptor = new TokenParseDescriptor<ShortcutScriptInput>()
			{
				TokenPrototype = new()
				{
					Key = "text-shortcuts-script-input",
				},
				SubTokenHandlers = new()
				{
					{ Syntax.TokenPrototypes.EntryList, entryListTokenHandler },
					{ Syntax.TokenPrototypes.ModuleOptions, moduleOptionsTokenHandler },
					{ Syntax.TokenPrototypes.ShortcutList, shortcutListTokenHandler },
				},
				PostParseAction = (ShortcutScriptInput result) =>
				{
					result = ShortcutProcessor.SanitizePlaintextShortcuts(result);
					result = ShortcutProcessor.GenerateTemplatedShortcuts(result);
					return result;
				},
			};

			return new TokenBodyParser<ShortcutScriptInput>(tokenParseDescriptor);
		}
	}
}
