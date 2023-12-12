﻿using Petrichor.AutoHotkeyScripts.Containers;
using Petrichor.AutoHotkeyScripts.LookUpTables;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.Text;

namespace Petrichor.AutoHotkeyScripts.Utilities
{
	public class AutoHotkeyScriptGenerator
	{
		private string outputFilePath = string.Empty;


		public string[] GenerateMacrosFromInput( Input input )
		{
			var macros = new List< string >();
			foreach ( var entry in input.Entries )
			{
				macros.AddRange( GenerateAllEntryMacrosFromTemplates( input.Templates, entry ) );
			}
			return macros.ToArray();
		}

		public void GenerateScript( string[] macros, string outputFile )
		{
			var outputFolder = GetNormalizedOutputFolder( outputFile );
			var outputFileName = GetNormalizedOutputFileName( outputFile );
			outputFilePath = $"{ outputFolder }{ outputFileName }";

			var taskMessage = $"generating output file \"{ outputFilePath }\"";
			Log.TaskStarted( taskMessage );
			
			try
			{
				Directory.CreateDirectory( outputFolder );
				WriteHeaderToFile();
				WriteMacrosToFile( macros );
			}
			catch ( Exception ex )
			{
				var errorMessage = $"failed to generate output file ({ outputFilePath })";
				Log.Error( errorMessage );
				throw new Exception( errorMessage, ex );
			}
			Log.TaskFinished( taskMessage );
		}


		private List< string > GenerateAllIdentityMacrosFromTemplates( string[] templates, Identity identity, string pronoun, string decoration )
		{
			var macros = new List< string >();
			foreach ( var template in templates )
			{
				macros.Add( GenerateIdentityMacroFromTemplate( template, identity, pronoun, decoration ) );
			}
			return macros;
		}

		private List< string > GenerateAllEntryMacrosFromTemplates( string[] templates, Entry entry )
		{
			var macros = new List< string >();
			foreach ( var identity in entry.Identities )
			{
				macros.AddRange( GenerateAllIdentityMacrosFromTemplates( templates, identity, entry.Pronoun, entry.Decoration ) );
			}
			return macros;
		}

		private string GenerateIdentityMacroFromTemplate( string template, Identity identity, string pronoun, string decoration )
		{
			var macro = template;
			Dictionary< string, string > fields = new Dictionary< string, string >()
			{
				{ "name", identity.Name },
				{ "tag", identity.Tag },
				{ "pronoun", pronoun },
				{ "decoration", decoration },
			 };
			foreach ( var marker in TemplateMarkers.LookUpTable )
			{
				macro = macro.Replace( $"`{ marker.Value }`", fields[ marker.Value ] );
			}
			return macro;
		}

		private string GetNormalizedOutputFolder( string outputFile )
		{
			var outputFolder = outputFile.GetDirectory();
			if ( outputFolder == string.Empty )
			{
				return ProjectDirectories.OutputDirectory;
			}
			return outputFolder;
		}

		private string GetNormalizedOutputFileName( string outputFile )
		{
			return $"{ outputFile.GetFileName().RemoveFileExtension() }.ahk";
		}

		private void WriteByteOrderMarkToFile()
		{
			var encoding = Encoding.UTF8;
			using ( FileStream stream = new FileStream( outputFilePath, FileMode.Create ) )
			{
				using ( BinaryWriter writer = new BinaryWriter( stream, encoding ) )
				{
					writer.Write( encoding.GetPreamble() );
				}
			}
		}

		private void WriteHeaderToFile()
		{
			var taskMessage = "writing header to output file";
			Log.TaskStarted( taskMessage );
			WriteByteOrderMarkToFile();
			var header = new string[]
			{
				$"; Generated with { AppInfo.AppName } v{ AppInfo.CurrentVersion }",
				"",
				"",
				"#SingleInstance Force",
				"",
			};
			WriteLinesToFile( header );
			Log.TaskFinished( taskMessage );
		}

		private void WriteLineToFile( string line = "" )
		{
			try
			{
				using ( StreamWriter writer = File.AppendText( outputFilePath ) )
				{
					writer.WriteLine( line );
				}
			}
			catch ( Exception ex )
			{
				var errorMessage = "failed to write line to output file";
				Log.Error( errorMessage );
				throw new FileLoadException( errorMessage, ex );
			}
		}

		private void WriteLinesToFile( string[] data )
		{
			var linesWritten = 0;
			foreach ( string line in data )
			{
				WriteLineToFile( line );
				++linesWritten;
			}
			Log.Info( $"wrote { linesWritten } lines to output file" );
		}

		private void WriteMacrosToFile( string[] macros )
		{
			var taskMessage = "writing macros to output file";
			Log.TaskStarted( taskMessage );
			WriteLinesToFile( macros );
			Log.TaskFinished( taskMessage );
		}
	}
}
