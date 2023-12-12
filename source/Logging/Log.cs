using System.Reflection;

using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		private static readonly string defaultLogFolder = $"{ Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) }/log/";
		private static readonly string defaultLogFileName = $"{ DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" ) }.log";
		private static LogMode mode = LogMode.Disabled;
		private static string logFolder = string.Empty;
		private static string logFileName = string.Empty;
		private static string logFilePath = string.Empty;


		public static void Disable()
		{
			mode = LogMode.Disabled;
		}

		public static void EnableBasic()
		{
			mode = LogMode.Basic;
		}

		public static void EnableVerbose()
		{
			mode = LogMode.Verbose;
		}

		public static void SetLogFileName( string filename )
		{
			logFileName = filename;
			SetLogFilePath();
		}

		public static void SetLogFolder( string folder )
		{
			logFolder = folder;
			var lastChar = folder[ folder.Length - 1 ];
			if ( lastChar != '\\' && lastChar != '/' )
			{
				logFolder += '/';
			}
			Directory.CreateDirectory( logFolder );
			SetLogFilePath();
		}

		public static void Write( string message = "" )
		{
			if ( mode == LogMode.Disabled || message == "" )
			{
				return;
			}

			if ( message != "\n" )
			{
				message = $"[{ DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) }] { message }";
			}

			if ( logFolder == "" )
			{
				SetLogFolder( defaultLogFolder );
			}
			if ( logFileName == "" )
			{
				SetLogFileName( defaultLogFileName );
			}

			using ( StreamWriter logFile = File.AppendText( logFilePath ) )
			{
				logFile.Write( message );
			}
			if ( mode == LogMode.Verbose )
			{
				Console.Write( message );
			}
		}

		public static void WriteLine( string message = "" )
		{
			Write( $"{ message }\n" );
		}


		private static void SetLogFilePath()
		{
			logFilePath = $"{ logFolder }{ logFileName }";
		}
	}
}
