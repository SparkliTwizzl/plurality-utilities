using PluralityUtilities.AutoHotkeyScripts.Containers;
using PluralityUtilities.AutoHotkeyScripts.Templates;
using PluralityUtilities.Common;
using PluralityUtilities.Common.Utilities;
using PluralityUtilities.Logging;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class AutoHotkeyScriptGenerator
	{
		private string _outputFolder = string.Empty;
		private string _outputFileName = string.Empty;
		private string _outputFilePath = string.Empty;


		public void GenerateScript(List<Person> people, string outputFile)
		{
			NormalizeOutputFile(outputFile);
			Log.WriteLineTimestamped($"started generating output file: {_outputFilePath}");
			Directory.CreateDirectory(_outputFolder);
			File.Create(_outputFilePath).Close();
			foreach (Person person in people)
			{
				Log.WriteLineTimestamped("started writing person to output file");
				WritePersonToFile(person);
				Log.WriteLineTimestamped("successfully wrote person to output file");
			}
			Log.WriteLineTimestamped("successfully generated output file");
		}


		private void NormalizeOutputFile(string outputFile)
		{
			_outputFolder = outputFile.GetDirectory();
			if (_outputFolder == string.Empty)
			{
				_outputFolder = ProjectDirectories.OutputDir;
			}
			_outputFileName = outputFile.GetFileName().RemoveFileExtension();
			_outputFilePath = $"{_outputFolder}{_outputFileName}.ahk";
		}

		private void WriteMacrosToFile(Identity identity, string pronoun, string decoration)
		{
			foreach (string template in MacroTemplates.Templates)
			{
				var macro = TemplateParser.ParseMacroFromTemplate(template, identity, pronoun, decoration);
				WriteLineToFile(macro);
				Log.WriteLineTimestamped($"wrote macro to output file: {macro}");
			}
			WriteLineToFile();
		}

		private void WritePersonToFile(Person person)
		{
			for (int i = 0; i < person.Identities.Count; ++i)
			{
				Log.WriteLineTimestamped($"started writing identity {i + 1} of {person.Identities.Count} to output file");
				var identity = person.Identities[i];
				WriteMacrosToFile(identity, person.Pronoun, person.Decoration);
				Log.WriteLineTimestamped($"successfully wrote identity to output file");
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
				Log.WriteLineTimestamped($"error: {errorMessage}");
				throw new FileLoadException(errorMessage, ex);
			}
		}
	}
}
