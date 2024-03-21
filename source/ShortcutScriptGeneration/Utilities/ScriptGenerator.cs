﻿using Petrichor.Common.Info;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using System.Text;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	public class ScriptGenerator
	{
		private ScriptInput Input { get; set; } = new();
		private string OutputFilePath { get; set; } = string.Empty;
		private int TotalLinesWritten { get; set; } = 0;


		public void Generate( ScriptInput input, string outputFile )
		{
			Input = input;

			var outputDirectory = GetNormalizedOutputDirectory( outputFile );
			var outputFileName = GetNormalizedOutputFileName( outputFile );
			OutputFilePath = $"{outputDirectory}{outputFileName}";

			var taskMessage = $"Generate output file \"{OutputFilePath}\"";
			Log.Start( taskMessage );

			try
			{
				_ = Directory.CreateDirectory( outputDirectory );
				WriteHeaderToFile();
				WriteMacrosToFile();
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new ScriptGenerationException( $"Failed to generate output file \"{OutputFilePath}\".", exception ) );
			}

			Log.Info( $"Wrote {TotalLinesWritten} total lines to output file." );
			Log.Finish( taskMessage );
		}


		private static string GetNormalizedOutputDirectory( string outputFile )
		{
			var outputDirectory = Path.GetDirectoryName( outputFile );
			if ( outputDirectory is null || outputDirectory == string.Empty )
			{
				return ProjectDirectories.OutputDirectory + @"\";
			}
			return outputDirectory + @"\";
		}

		private static string GetNormalizedOutputFileName( string outputFile )
			=> Path.GetFileNameWithoutExtension( outputFile ) + ".ahk";

		private void WriteByteOrderMarkToFile()
		{
			var encoding = Encoding.UTF8;
			using var stream = new FileStream( OutputFilePath, FileMode.Create );
			using var writer = new BinaryWriter( stream, encoding );
			writer.Write( encoding.GetPreamble() );
			Log.Info( "Wrote byte order mark to output file." );
		}

		private void WriteConstantsToFile()
		{
			var lines = new string[]
			{
				"; constants used for icon handling",
				"FREEZE_ICON := true",
				"ID_FILE_SUSPEND := 65305",
				"ID_TRAY_SUSPEND := 65404",
				"SUSPEND_OFF := 0",
				"SUSPEND_ON := 1",
				"SUSPEND_TOGGLE := -1",
				"WM_COMMAND := 0x111",
				"",
				"",
			};
			WriteLinesToFile( lines, " (Constants needed for script execution)" );
		}

		private void WriteControlShortcutsToFile()
		{
			if ( Input.ModuleOptions.ReloadShortcut == string.Empty && Input.ModuleOptions.SuspendShortcut == string.Empty )
			{
				return;
			}

			var lines = new List<string>
			{
				"; script reload / suspend shortcut(s)",
				"#SuspendExempt true"
			};

			if ( Input.ModuleOptions.ReloadShortcut != string.Empty )
			{
				lines.Add( $"{Input.ModuleOptions.ReloadShortcut}::Reload()" );
			}

			if ( Input.ModuleOptions.SuspendShortcut != string.Empty )
			{
				lines.Add( $"{Input.ModuleOptions.SuspendShortcut}::Suspend( SUSPEND_TOGGLE)" );
			}

			lines.Add( "#SuspendExempt false" );
			lines.Add( "" );
			lines.Add( "" );
			WriteLinesToFile( lines.ToArray(), " (Script reload/suspend shortcuts)" );
		}

		private void WriteControlStatementsToFile()
		{
			var lines = new string[]
			{
				"#Requires AutoHotkey v2.0",
				"#SingleInstance Force",
				"",
			};
			WriteLinesToFile( lines, " (AutoHotkey control statements)" );
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
			WriteLinesToFile( lines, " (\"Generated by\" message)" );
		}

		private void WriteHeaderToFile()
		{
			var taskMessage = "Write header to output file";
			Log.Start( taskMessage );
			WriteByteOrderMarkToFile();
			WriteGeneratedByMessageToFile();
			WriteControlStatementsToFile();
			WriteIconFilePathsToFile();
			WriteConstantsToFile();
			WriteIconHandlingToFile();
			WriteControlShortcutsToFile();
			Log.Info( $"Wrote {TotalLinesWritten} total lines to output file header." );
			Log.Finish( taskMessage );
		}

		private void WriteIconFilePathsToFile()
		{
			var lines = new string[]
			{
				$"defaultIcon := { Input.ModuleOptions.DefaultIconFilePath }",
				$"suspendIcon := { Input.ModuleOptions.SuspendIconFilePath }",
				"",
				"",
			};
			WriteLinesToFile( lines, " (Icon filepaths)" );
		}

		private void WriteIconHandlingToFile()
		{
			var lines = new string[]
			{
				"; icon handling",
				"; based on code by ntepa on autohotkey.com/boards: https://www.autohotkey.com/boards/viewtopic.php?p=497349#p497349",
				"SuspendC := Suspend.GetMethod( \"Call\")",
				"Suspend.DefineProp( \"Call\",",
				"\t{",
				"\t\tCall:( this, mode := SUSPEND_TOGGLE ) => ( SuspendC( this, mode ), OnSuspend( A_IsSuspended ))",
				"\t})",
				"OnMessage( WM_COMMAND, OnSuspendMsg)",
				"OnSuspendMsg( wparam, *)",
				"{",
				"\tif ( wparam = ID_FILE_SUSPEND || wparam = ID_TRAY_SUSPEND)",
				"\t{",
				"\t\tOnSuspend( !A_IsSuspended)",
				"\t}",
				"}",
				"",
				"OnSuspend( mode)",
				"{",
				"\tscriptIcon := SelectIcon( mode)",
				"\tSetIcon( scriptIcon)",
				"}",
				"",
				"SelectIcon( suspendMode)",
				"{",
				"\tif ( suspendMode = SUSPEND_ON)",
				"\t{",
				"\t\treturn suspendIcon",
				"\t}",
				"\telse if ( suspendMode = SUSPEND_OFF)",
				"\t{",
				"\t\treturn defaultIcon",
				"\t}",
				"\treturn \"\"",
				"}",
				"",
				"SetIcon( scriptIcon)",
				"{",
				"\tif ( FileExist( scriptIcon ))",
				"\t{",
				"\t\tTraySetIcon( scriptIcon,, FREEZE_ICON)",
				"\t}",
				"}",
				"",
				"SetIcon( defaultIcon)",
				"",
				"",
			};
			WriteLinesToFile( lines, " (Icon handling logic)" );
		}

		private void WriteLineToFile( string line = "" )
		{
			try
			{
				using var writer = File.AppendText( OutputFilePath );
				writer.WriteLine( line );
				++TotalLinesWritten;
			}
			catch ( Exception exception )
			{
				ExceptionLogger.LogAndThrow( new FileLoadException( "Failed to write line to output file.", exception ) );
			}
		}

		private void WriteLinesToFile( string[] lines, string message = "" )
		{
			var linesWritten = 0;
			foreach ( var line in lines )
			{
				WriteLineToFile( line );
				++linesWritten;
			}
			Log.Info( $"Wrote {linesWritten} lines to output file{message}." );
		}

		private void WriteMacrosToFile()
		{
			var taskMessage = "Write macros to output file";
			Log.Start( taskMessage );
			var lines = new List<string>
			{
				"; macros generated from entries and templates",
			};
			lines.AddRange( Input.Macros );
			WriteLinesToFile( lines.ToArray() );
			Log.Finish( taskMessage );
		}
	}
}
