using Petrichor.AutoHotkeyScripts.Containers;
using Petrichor.AutoHotkeyScripts.LookUpTables;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using System.Text;

namespace Petrichor.AutoHotkeyScripts.Utilities
{
	public class AutoHotkeyScriptGenerator
	{
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
			var outputFilePath = $"{ outputFolder }{ outputFileName }";

			Log.WriteLineTimestamped( $"STARTED: generating output file ({ outputFilePath })" );
			try
			{
				Directory.CreateDirectory( outputFolder );
				WriteHeaderToFile( outputFilePath );
				WriteLinesToFile( outputFilePath, macros );
			}
			catch ( Exception ex )
			{
				var errorMessage = $"failed to generate output file ({ outputFilePath })";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new Exception( errorMessage, ex );
			}
			Log.WriteLineTimestamped( "FINISHED: generating output file" );
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

		private void WriteByteOrderMarkToFile( string outputFilePath )
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

		private void WriteHeaderToFile( string outputFilePath )
		{
			Log.WriteLineTimestamped( "STARTED: writing header to output file" );
			WriteByteOrderMarkToFile( outputFilePath );
			var header = new string[]
			{
				"#SingleInstance Force",
				"",
			};
			WriteLinesToFile( outputFilePath, header );
			Log.WriteLineTimestamped( "FINISHED: writing header to output file" );
		}

		private void WriteLineToFile( string outputFilePath, string line = "" )
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
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new FileLoadException( errorMessage, ex );
			}
		}

		private void WriteLinesToFile( string outputFilePath, string[] data )
		{
			var linesWritten = 0;
			foreach ( string line in data )
			{
				WriteLineToFile( outputFilePath, line );
				++linesWritten;
			}
			Log.WriteLineTimestamped( $"wrote { linesWritten } lines to output file" );
		}
	}
}
