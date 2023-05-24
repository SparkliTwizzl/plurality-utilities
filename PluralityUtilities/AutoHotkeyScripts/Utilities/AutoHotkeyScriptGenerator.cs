using PluralityUtilities.Common;
using PluralityUtilities.Common.Utilities;
using PluralityUtilities.Logging;
using System.Text;

namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class AutoHotkeyScriptGenerator
	{
		public static void GenerateScript( string[] macros, string outputFile )
		{
			var outputFolder = GetNormalizedOutputFolder( outputFile );
			var outputFileName = GetNormalizedOutputFileName( outputFile );
			var outputFilePath = $"{ outputFolder }{ outputFileName }";

			Log.WriteLineTimestamped( $"generating output file ({ outputFilePath })..." );
			Directory.CreateDirectory( outputFolder );

			var encoding = Encoding.UTF8;
			using ( FileStream stream = new FileStream( outputFilePath, FileMode.Create ) )
			{
				using ( BinaryWriter writer = new BinaryWriter( stream, encoding ) )
				{
					writer.Write( encoding.GetPreamble() );
				}
			}

			WriteHeaderToFile( outputFilePath );
			WriteLinesToFile( outputFilePath, macros );

			Log.WriteLineTimestamped( $"successfully generated output file ({ outputFilePath })" );
		}


		private static string GetNormalizedOutputFolder( string outputFile )
		{
			var outputFolder = outputFile.GetDirectory();
			if ( outputFolder == string.Empty )
			{
				return ProjectDirectories.OutputDir;
			}
			return outputFolder;
		}

		private static string GetNormalizedOutputFileName( string outputFile )
		{
			return $"{ outputFile.GetFileName().RemoveFileExtension() }.ahk";
		}

		private static void WriteHeaderToFile( string outputFilePath )
		{
			var header = new string[]
			{
				"#SingleInstance Force",
				""
			 };
			WriteLinesToFile( outputFilePath, header );
		}

		private static void WriteLineToFile( string outputFilePath, string line = "" )
		{
			try
			{
				using ( StreamWriter writer = File.AppendText( outputFilePath ) )
				{
					writer.WriteLine( line );
					Log.WriteLineTimestamped( $"wrote line to output file: { line }" );
				}
			}
			catch ( Exception ex )
			{
				var errorMessage = "failed to write to output file";
				Log.WriteLineTimestamped( $"error: { errorMessage }" );
				throw new FileLoadException( errorMessage, ex );
			}
		}

		private static void WriteLinesToFile( string outputFilePath, string[] data )
		{
			foreach ( string line in data )
			{
				WriteLineToFile( outputFilePath, line );
			}
		}
	}
}
