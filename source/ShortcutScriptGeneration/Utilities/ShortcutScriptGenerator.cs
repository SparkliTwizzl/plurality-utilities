﻿using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ShortcutScriptGenerator : IShortcutScriptGenerator
	{
		private string outputFilePath = string.Empty;


		private ShortcutScriptInput Input { get; set; }


		public ShortcutScriptGenerator(ShortcutScriptInput input)
		{
			Input = input;
		}


		public void GenerateScript(string outputFile)
		{
			var outputDirectory = GetNormalizedOutputDirectory(outputFile);
			var outputFileName = GetNormalizedOutputFileName(outputFile);
			outputFilePath = $"{outputDirectory}{outputFileName}";

			var taskMessage = $"generating output file \"{outputFilePath}\"";
			Log.TaskStarted(taskMessage);

			try
			{
				Directory.CreateDirectory(outputDirectory);
				WriteHeaderToFile();
				WriteMacrosToFile();
			}
			catch (Exception ex)
			{
				var errorMessage = $"failed to generate output file ({outputFilePath})";
				Log.Error(errorMessage);
				throw new Exception(errorMessage, ex);
			}
			Log.TaskFinished(taskMessage);
		}

		private string GetNormalizedOutputDirectory(string outputFile)
		{
			var outputDirectory = outputFile.GetDirectory();
			if (outputDirectory == string.Empty)
			{
				return ProjectDirectories.OutputDirectory;
			}
			return outputDirectory;
		}

		private string GetNormalizedOutputFileName(string outputFile)
		{
			return $"{outputFile.GetFileName().RemoveFileExtension()}.ahk";
		}


		private void WriteByteOrderMarkToFile()
		{
			var encoding = Encoding.UTF8;
			using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
			{
				using (BinaryWriter writer = new BinaryWriter(stream, encoding))
				{
					writer.Write(encoding.GetPreamble());
				}
			}
		}

		private void WriteControlStatementsToFile()
		{
			var lines = new string[]
			{
				"#Requires AutoHotkey v2.0",
				"#SingleInstance Force",
				"",
			};
			WriteLinesToFile(lines);
		}

		private void WriteGeneratedByMessageToFile()
		{
			var lines = new string[]
			{
				$"; Generated by { AppInfo.AppNameAndVersion } AutoHotkey shortcut script generator",
				"; https://github.com/SparkliTtwizzl/petrichor",
				"",
				"",
			};
			WriteLinesToFile(lines);
		}

		private void WriteHeaderToFile()
		{
			var taskMessage = "writing header to output file";
			Log.TaskStarted(taskMessage);
			WriteByteOrderMarkToFile();
			WriteGeneratedByMessageToFile();
			WriteControlStatementsToFile();
			WriteIconFilePathsToFile();
			WriteSetIconFunctionToFile();
			Log.TaskFinished(taskMessage);
		}

		private void WriteIconFilePathsToFile()
		{
			var lines = new string[]
			{
				$"defaultIcon := \"{ Input.Metadata.DefaultIconPath }\"",
				$"suspendIcon := \"{ Input.Metadata.SuspendIconPath }\"",
				"",
				"",
			};
			WriteLinesToFile(lines);
		}

		private void WriteSetIconFunctionToFile()
		{
			var lines = new string[]
			{
				"#SuspendExempt true",
				"SetIcon()",
				"{",
				"	scriptIcon := A_IsSuspended ? suspendIcon : defaultIcon",
				"	TraySetIcon(scriptIcon,, true)",
				"}",
				"#SuspendExempt false",
				"",
				"SetIcon()",
				"",
				"",
			};
			WriteLinesToFile(lines);
		}

		private void WriteLineToFile(string line = "")
		{
			try
			{
				using (StreamWriter writer = File.AppendText(outputFilePath))
				{
					writer.WriteLine(line);
				}
			}
			catch (Exception ex)
			{
				var errorMessage = "failed to write line to output file";
				Log.Error(errorMessage);
				throw new FileLoadException(errorMessage, ex);
			}
		}

		private void WriteLinesToFile(string[] lines)
		{
			var linesWritten = 0;
			foreach (string line in lines)
			{
				WriteLineToFile(line);
				++linesWritten;
			}
			Log.Info($"wrote {linesWritten} lines to output file");
		}

		private void WriteMacrosToFile()
		{
			var taskMessage = "writing macros to output file";
			Log.TaskStarted(taskMessage);
			WriteLinesToFile(Input.Macros);
			Log.TaskFinished(taskMessage);
		}
	}
}
