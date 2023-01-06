using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Enums;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class AkfFileParser
	{
		public List<Person> People { get; private set; } = new List<Person>();


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
				throw new DuplicateFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank decoration field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankFieldException(errorMessage);
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
					var errorMessage = "input file contains invalid data: an unknown character was read when '{' was expected";
					Log.WriteLineTimestamped($"error: {errorMessage}");
					throw new UnknownCharacterException(errorMessage);
				}
			}
		}

		private LineTypes ParseLine(string line, ref Person person)
		{
			line =line.TrimStart();
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
					var unrecognizedChar = "input file contains invalid data: an unrecognized character was read at the start of a line";
					Log.WriteLineTimestamped($"error: {unrecognizedChar}");
					throw new UnknownCharacterException(unrecognizedChar);
			}
		}

		private void ParseName(string line, ref Identity identity)
		{
			var nameStart = line.IndexOf('#') + 1;
			var nameEnd = line.LastIndexOf('#') - 1;
			if (nameEnd - nameStart < 1)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankFieldException(errorMessage);
			}
			identity.Name = line.Substring(nameStart, nameEnd - nameStart);
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
						return person;
					case LineTypes.Unknown:
						var unknownChar = "input file contains invalid data: a line started with an unknown character";
						Log.WriteLineTimestamped($"error: {unknownChar}");
						throw new UnknownCharacterException(unknownChar);
					default:
						break;
				}
			}
			var lastEntryNotClosed = "input file contains invalid data: last entry was not closed";
			Log.WriteLineTimestamped($"error: {lastEntryNotClosed}");
			throw new EntryNotClosedException(lastEntryNotClosed);
		}

		private void ParsePronoun(string line, ref Person person)
		{
			if (person.Pronoun != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new DuplicateFieldException(errorMessage);
			}
			if (line.Length < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank pronoun field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankFieldException(errorMessage);
			}
			person.Pronoun = line.Substring(1, line.Length - 1);
		}

		private void ParseTag(string line, ref Identity identity)
		{
			var tagStart = line.IndexOf('@');
			if (tagStart < 0)
			{
				var errorMessage = "input file contains invalid data: an entry contained a name field without a paired tag field";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankFieldException(errorMessage);
			}
			var lastSpace = line.LastIndexOf(' ');
			if (lastSpace > tagStart)
			{
				var errorMessage = "input file contains invalid data: tag fields cannot contain spaces";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new BlankFieldException(errorMessage);
			}
			identity.Tag = line.Substring(tagStart, line.Length - tagStart);
		}

		private string[] ReadDataFromFile(string inputFilePath)
		{
			try
			{
				var data = File.ReadAllLines(inputFilePath);
				Log.WriteLineTimestamped("successfully read data from input file");
				return data;
			}
			catch
			{
				var errorMessage = "failed to read data from input file";
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new FileNotFoundException(errorMessage);
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
