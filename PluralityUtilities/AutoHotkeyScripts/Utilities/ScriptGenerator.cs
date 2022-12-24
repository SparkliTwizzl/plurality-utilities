using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Templates;
using PluralityUtilities.Logging;
using System.Text;

namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class ScriptGenerator
	{
		private string _outputFilePath = string.Empty;


		public void Generate(List<Person> people, string outputFilePath)
		{
			Log.WriteLine($"started generating output file: {outputFilePath}");
			_outputFilePath = outputFilePath;
			File.Create(_outputFilePath).Close();
			foreach (Person person in people)
			{
				WritePersonToFile(person);
			}
			Log.WriteLine("successfully generated output file");
		}


		private string CreateMacroFromTemplate(string template, string name, string tag, string pronoun)
		{
			StringBuilder macro = new StringBuilder();
			foreach (char c in template)
			{
				switch (c)
				{
					case '#':
						macro.Append(name);
						break;
					case '@':
						macro.Append(tag);
						break;
					case '$':
						macro.Append(pronoun);
						break;
					default:
						macro.Append(c);
						break;
				}
			}
			return macro.ToString();
		}

		private void WriteMacrosToFile(string name, string tag, string pronoun)
		{
			foreach (string template in MacroTemplates.Templates)
			{
				var macro = CreateMacroFromTemplate(template, name, tag, pronoun);
				WriteLineToFile(macro);
				Log.WriteLine($"wrote macro to output file: {macro}");
			}
			WriteLineToFile();
		}

		private void WritePersonToFile(Person person)
		{
			for (int i = 0; i < person.Identities.Count; ++i)
			{
				Log.WriteLine($"started writing person {i + 1} of {person.Identities.Count} to output file");
				var identity = person.Identities[i];
				WriteMacrosToFile(identity.Name, identity.Tag, person.Pronoun);
				Log.WriteLine($"successfully wrote person {i + 1} of {person.Identities.Count} to output file");
			}
		}

		private void WriteLineToFile(string line = "")
		{
			try
			{
				using (StreamWriter writer = File.AppendText(_outputFilePath))
				{
					writer.WriteLine(line);
				}
			}
			catch (Exception ex)
			{
				var errorMessage = "failed to write to output file";
				Log.WriteLine($"error: {errorMessage}");
				throw new FileLoadException(errorMessage, ex);
			}
		}
	}
}
