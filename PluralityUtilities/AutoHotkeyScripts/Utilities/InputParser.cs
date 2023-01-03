using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Enums;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class InputParser
	{
		public List<Person> People { get; private set; } = new List<Person>();


		// throws BlankInputFieldException if file contains a field with no value
		// throws DuplicateInputFieldException if file contains an entry with more than one decoration field
		// throws DuplicateInputFieldException if file contains an entry with more than one pronoun field
		// throws InputEntryNotClosedException if file contains an entry that is not closed
		// throws FileNotFoundException if file data could not be read
		// throws InvalidArgumentException if file extension is missing or not recognized
		// throws InvalidInputFieldException if file contains a tag field with spaces in it
		// throws MissingInputFieldException if file contains an entry with no identity fields
		// throws MissingInputFieldException if file contains an identity field with no name field
		// throws MissingInputFieldException if file contains an identity field with no tag field
		// throws UnexpectedCharacterException if file contains a line that starts with an unexpected character
		public void ParseFile(string inputFilePath)
		{
			Log.WriteLineTimestamped("started parsing input file: " + inputFilePath);
			VerifyFileExtension(inputFilePath);
			var inputData = ReadDataFromFile(inputFilePath);
			ParseInputData(inputData);
			Log.WriteLineTimestamped("finished parsing input file");
		}


		private void ParseDecoration(string line, ref Person person)
		{
			if (person.Decoration != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one decoration field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new DuplicateInputFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank decoration field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankInputFieldException(errorMessage);
			}
			person.Decoration = line.Substring(1, line.Length - 1);
		}

		private void ParseIdentity(string line, ref Person person)
		{
			Identity identity = new Identity();
			ParseName(line, ref identity);
			ParseTag(line, ref identity);
			person.Identities.Add(identity);
		}

		private void ParseInputData(string[] data)
		{
			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] == "{")
				{
					++i;
					var person = ParsePerson(data, ref i);
					People.Add(person);
					Log.WriteTimestamped("successfully parsed entry: names/tags [");
					foreach (Identity identity in person.Identities)
					{
						Log.Write($"{identity.Name}/{identity.Tag}, ");
					}
					Log.WriteLine($"], pronoun [{person.Pronoun}], decoration [{person.Decoration}]");
				}
				else
				{
					var errorMessage = "input file contains invalid data: an unexpected character was read when '{' was expected";
					Log.WriteLineTimestamped($"error: {errorMessage}");
					throw new UnexpectedCharacterException(errorMessage);
				}
			}
		}

		private LineTypes ParseLine(string line, ref Person person)
		{
			line = line.TrimStart();
			var firstChar = line[0];
			switch (firstChar)
			{
				case '}':
					return LineTypes.EntryEnd;
				case '%':
					ParseIdentity(line, ref person);
					return LineTypes.Name;
				case '$':
					ParsePronoun(line, ref person);
					return LineTypes.Pronoun;
				case '&':
					ParseDecoration(line, ref person);
					return LineTypes.Decoration;
				default:
					return LineTypes.Unexpected;
			}
		}

		private void ParseName(string line, ref Identity identity)
		{
			var fieldStart = line.IndexOf('#');
			var fieldEnd = line.LastIndexOf('#');
			if (fieldStart < 0)
			{
				var errorMessage = "input file contains invalid data: an entry had no name fields";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new MissingInputFieldException(errorMessage);
			}
			var name = line.Substring(fieldStart + 1, fieldEnd - (fieldStart + 1));
			if (name.Length < 1)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankInputFieldException(errorMessage);
			}
			identity.Name = name;
		}

		private Person ParsePerson(string[] data, ref int index)
		{
			Log.WriteLineTimestamped("started parsing next entry");
			var person = new Person();
			for (; index < data.Length; ++index)
			{
				var result = ParseLine(data[index], ref person);
				switch (result)
				{
					case LineTypes.EntryEnd:
						if (person.Identities.Count < 1)
						{
							var noIdentities = "input file contains invalid data: an entry did not contain any identity fields";
							Log.WriteLineTimestamped($"error: {noIdentities}");
							throw new MissingInputFieldException(noIdentities);
						}
						return person;
					case LineTypes.Unexpected:
						var unexpectedChar = "input file contains invalid data: a line started with a character that was not expected at this time";
						Log.WriteLineTimestamped($"error: {unexpectedChar}");
						throw new UnexpectedCharacterException(unexpectedChar);
					default:
						break;
				}
			}
			var lastEntryNotClosed = "input file contains invalid data: last entry was not closed";
			Log.WriteLineTimestamped($"error: {lastEntryNotClosed}");
			throw new InputEntryNotClosedException(lastEntryNotClosed);
		}

		private void ParsePronoun(string line, ref Person person)
		{
			if (person.Pronoun != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new DuplicateInputFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank pronoun field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankInputFieldException(errorMessage);
			}
			person.Pronoun = line.Substring(1, line.Length - 1);
		}

		private void ParseTag(string line, ref Identity identity)
		{
			var fieldStart = line.IndexOf('@');
			if (fieldStart < 0)
			{
				var errorMessage = "input file contains invalid data: an entry contained an identity field without a tag field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new MissingInputFieldException(errorMessage);
			}
			var lastSpace = line.LastIndexOf(' ');
			if (lastSpace >= fieldStart)
			{
				var errorMessage = "input file contains invalid data: tag fields cannot contain spaces";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new InvalidInputFieldException(errorMessage);
			}
			var tag = line.Substring(fieldStart + 1, line.Length - (fieldStart + 1));
			if (tag.Length < 1)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank tag field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankInputFieldException(errorMessage);
			}
			identity.Tag = tag;
		}

		private string[] ReadDataFromFile(string inputFilePath)
		{
			try
			{
				var data = File.ReadAllLines(inputFilePath);
				Log.WriteLineTimestamped("successfully read data from input file");
				return data;
			}
			catch (Exception ex)
			{
				var errorMessage = "failed to read data from input file";
				Log.WriteLineTimestamped($"error: {errorMessage}; {ex.Message}");
				throw new FileNotFoundException(errorMessage, ex);
			}
		}

		private void VerifyFileExtension(string inputFilePath)
		{
			if (inputFilePath.Substring(inputFilePath.Length - 4, 4) != ".akf")
			{
				var errorMessage = "input file path was not a .akf file";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new InvalidArgumentException(errorMessage);
			}
			Log.WriteLineTimestamped("input file path was a .akf file");
		}
	}
}
