using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Enums;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.Common.Enums;
using Petrichor.Logging;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptEntryParser : IShortcutScriptEntryParser
	{
		private StringTokenParser TokenParser = new StringTokenParser();


		public ShortcutScriptEntryParser() { }


		public ShortcutScriptEntry[] ParseEntriesFromData(string[] data, ref int i)
		{
			var taskMessage = "parsing entries from input data";
			Log.TaskStarted(taskMessage);

			var entries = new List<ShortcutScriptEntry>();
			var expectedTokens = new string[]
			{
				"%",
				"$",
				"&",
			};

			string? errorMessage;
			for (; i < data.Length; ++i)
			{
				var isParsingFinished = false;
				var trimmedLine = data[i].Trim();
				var firstChar = trimmedLine.FirstOrDefault();
				var token = TokenParser.ParseToken(firstChar.ToString(), expectedTokens);
				switch (token.Qualifier)
				{
					case StringTokenQualifiers.BlankLine:
						{
							break;
						}

					case StringTokenQualifiers.CloseBracket:
						{
							if (TokenParser.IndentLevel == 0)
							{
								isParsingFinished = true;
							}
							break;
						}

					case StringTokenQualifiers.Unknown:
						{
							errorMessage = $"parsing entries failed at token # {i} :: input file contains invalid data: a line started with a character ( \"{firstChar}\" ) that was not expected at this time";
							Log.Error(errorMessage);
							throw new UnexpectedCharacterException(errorMessage);
						}

					default:
						{
							if (TokenParser.IndentLevel > 1)
							{
								++i;
								var entry = ParseEntry(data, ref i);
								entries.Add(entry);
								Log.WriteWithTimestamp("parsed entry: names/tags [");
								foreach (ShortcutScriptIdentity identity in entry.Identities)
								{
									Log.Write($"{identity.Name}/{identity.Tag}, ");
								}
								Log.WriteLine($"], pronoun [{entry.Pronoun}], decoration [{entry.Decoration}]");
							}
							break;
						}
				}
				if (isParsingFinished)
				{
					break;
				}
			}
			if (TokenParser.IndentLevel > 0)
			{
				errorMessage = "input file contains invalid data: an entry was not closed";
				Log.Error(errorMessage);
				throw new InputEntryNotClosedException(errorMessage);
			}

			Log.TaskFinished(taskMessage);
			return entries.ToArray();
		}


		private void ParseDecoration(string line, ref ShortcutScriptEntry entry)
		{
			if (entry.Decoration != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one decoration field";
				Log.Error(errorMessage);
				throw new DuplicateInputFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank decoration field";
				Log.Error(errorMessage);
				throw new BlankInputFieldException(errorMessage);
			}
			entry.Decoration = line.Substring(1, line.Length - 1);
		}

		private void ParseIdentity(string line, ref ShortcutScriptEntry entry)
		{
			ShortcutScriptIdentity identity = new ShortcutScriptIdentity();
			ParseName(line, ref identity);
			ParseTag(line, ref identity);
			entry.Identities.Add(identity);
		}

		private ShortcutScriptEntryLineTypes ParseLine(string line, ref ShortcutScriptEntry entry)
		{
			line = line.TrimStart();
			var firstChar = line[0];
			switch (firstChar)
			{
				case '{':
					{
						return ShortcutScriptEntryLineTypes.EntryStart;
					}

				case '}':
					{
						return ShortcutScriptEntryLineTypes.EntryEnd;
					}

				case '%':
					{
						ParseIdentity(line, ref entry);
						return ShortcutScriptEntryLineTypes.Identity;
					}

				case '$':
					{
						ParsePronoun(line, ref entry);
						return ShortcutScriptEntryLineTypes.Pronoun;
					}

				case '&':
					{
						ParseDecoration(line, ref entry);
						return ShortcutScriptEntryLineTypes.Decoration;
					}

				default:
					{
						return ShortcutScriptEntryLineTypes.Unknown;
					}
			}
		}

		private void ParseName(string line, ref ShortcutScriptIdentity identity)
		{
			var fieldStart = line.IndexOf('#');
			var fieldEnd = line.LastIndexOf('#');
			if (fieldStart < 0)
			{
				var errorMessage = "input file contains invalid data: an entry had no name fields";
				Log.Error(errorMessage);
				throw new MissingInputFieldException(errorMessage);
			}
			var name = line.Substring(fieldStart + 1, fieldEnd - (fieldStart + 1));
			if (name.Length < 1)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.Error(errorMessage);
				throw new BlankInputFieldException(errorMessage);
			}
			identity.Name = name;
		}

		private ShortcutScriptEntry ParseEntry(string[] data, ref int i)
		{
			var taskMessage = "parsing entry";
			Log.TaskStarted(taskMessage);

			var entry = new ShortcutScriptEntry();
			var errorMessage = string.Empty;
			for (; i < data.Length; ++i)
			{
				var line = data[i];
				var lineType = ParseLine(line, ref entry);
				switch (lineType)
				{
					case ShortcutScriptEntryLineTypes.EntryEnd:
						{
							if (entry.Identities.Count < 1)
							{
								errorMessage = $"parsing entries failed at token # {i} :: input file contains invalid data: an entry did not contain any identity fields";
								Log.Error(errorMessage);
								throw new MissingInputFieldException(errorMessage);
							}
							--TokenParser.IndentLevel;
							Log.TaskFinished(taskMessage);
							return entry;
						}

					case ShortcutScriptEntryLineTypes.Unknown:
						{
							var unexpectedChar = line.Trim()[0];
							errorMessage = $"parsing entries failed at token # {i} :: input file contains invalid data: a line started with a character ( \"{unexpectedChar}\" ) that was not expected at this time";
							Log.Error(errorMessage);
							throw new UnexpectedCharacterException(errorMessage);
						}

					default:
						{
							break;
						}
				}
			}
			errorMessage = "input file contains invalid data: last entry was not closed";
			Log.Error(errorMessage);
			throw new InputEntryNotClosedException(errorMessage);
		}

		private void ParsePronoun(string line, ref ShortcutScriptEntry entry)
		{
			if (entry.Pronoun != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun field";
				Log.Error(errorMessage);
				throw new DuplicateInputFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank pronoun field";
				Log.Error(errorMessage);
				throw new BlankInputFieldException(errorMessage);
			}
			entry.Pronoun = line.Substring(1, line.Length - 1);
		}

		private void ParseTag(string line, ref ShortcutScriptIdentity identity)
		{
			var fieldStart = line.IndexOf('@');
			if (fieldStart < 0)
			{
				var errorMessage = "input file contains invalid data: an entry contained an identity field without a tag field";
				Log.Error(errorMessage);
				throw new MissingInputFieldException(errorMessage);
			}
			var lastSpace = line.LastIndexOf(' ');
			if (lastSpace >= fieldStart)
			{
				var errorMessage = "input file contains invalid data: tag fields cannot contain spaces";
				Log.Error(errorMessage);
				throw new InvalidInputFieldException(errorMessage);
			}
			var tag = line.Substring(fieldStart + 1, line.Length - (fieldStart + 1));
			if (tag.Length < 1)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank tag field";
				Log.Error(errorMessage);
				throw new BlankInputFieldException(errorMessage);
			}
			identity.Tag = tag;
		}
	}
}
