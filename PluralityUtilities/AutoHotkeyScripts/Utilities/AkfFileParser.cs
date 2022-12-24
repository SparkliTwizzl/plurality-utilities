using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Enums;
using PluralityUtilities.AutoHotkeyScripts.Exceptions;
using PluralityUtilities.Logging;
using System;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class AkfFileParser
	{
		public List<Person> People { get; private set; } = new List<Person>();


		public void ParseFile(string inputFilePath)
		{
			Log.WriteLine("started parsing input file: " + inputFilePath);
			VerifyFileExtension(inputFilePath);
			var inputData = ReadDataFromFile(inputFilePath);
			ParseInputData(inputData);
			Log.WriteLine("finished parsing input file");
		}


		private void ParseInputData(string[] data)
		{
			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] == "{")
				{
					var person = ParsePerson(data, ref i);
					People.Add(person);
					Log.Write("successfully parsed entry with names/tags [");
					for (int n = 0; n < person.Names.Count; ++n)
					{
						Log.Write($"{person.Names[n]}/{person.Tags[n]}, ");
					}
					Log.WriteLine($"] and pronoun [{person.Pronoun}]");
				}
				else
				{
					var errorMessage = "input file contains invalid data: an unknown character was read when '{' was expected";
					Log.WriteLine($"error: {errorMessage}");
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
				case '#':
					ParseName(line, ref person);
					return LineTypes.Name;
				case '$':
					ParsePronoun(line, ref person);
					return LineTypes.Pronoun;
				default:
					var unrecognizedChar = "input file contains invalid data: an unrecognized character was read at the start of a line";
					Log.WriteLine($"error: {unrecognizedChar}");
					throw new UnknownCharacterException(unrecognizedChar);
			}
		}

		private void ParseName(string line, ref Person person)
		{
			var nameEnd = line.LastIndexOf('#') - 1;
			if (nameEnd < 2)
			{
				var errorMessage = "input file contains invalid data: an entry contained a blank name field";
				Log.WriteLine($"error: {errorMessage}");
				throw new BlankNameException(errorMessage);
			}
			person.Names.Add(line.Substring(1, nameEnd));
		}

		private Person ParsePerson(string[] data, ref int index)
		{
			Log.WriteLine("started parsing next entry");
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
						Log.WriteLine($"error: {unknownChar}");
						throw new UnknownCharacterException(unknownChar);
					default:
						break;
				}
			}
			var lastEntryNotClosed = "input file contains invalid data: last entry was not closed";
			Log.WriteLine($"error: {lastEntryNotClosed}");
			throw new EntryNotClosedException(lastEntryNotClosed);
		}

		private void ParsePronoun(string line, ref Person person)
		{
			if (person.Pronoun != string.Empty)
			{
				var errorMessage = "input file contains invalid data: an entry contained more than one pronoun line";
				Log.WriteLine($"error: {errorMessage}");
				throw new TooManyPronounsException(errorMessage);
			}
			person.Pronoun = line.Substring(1, line.Length - 1);
		}

		private void VerifyFileExtension(string inputFilePath)
		{
			if (inputFilePath.Substring(inputFilePath.Length - 5, 4) != ".akf")
			{
				var errorMessage = "input file path was not a .akf file";
				Log.WriteLine($"error: {errorMessage}");
				throw new InvalidArgumentException(errorMessage);
			}
			Log.WriteLine("input file path was a .akf file");
		}

		private string[] ReadDataFromFile(string inputFilePath)
		{
			try
			{
				var data = File.ReadAllLines(inputFilePath);
				Log.WriteLine("successfully read data from input file");
				return data;
			}
			catch
			{
				var errorMessage = "failed to read data from input file";
				Log.WriteLine($"error: {errorMessage}");
				throw new FileNotFoundException(errorMessage);
			}
		}
	}
}
