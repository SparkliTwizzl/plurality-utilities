using System.Reflection;

using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		private const ConsoleColor defaultConsoleBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor defaultConsoleForegroundColor = ConsoleColor.White;
		private static readonly string defaultLogFolder = $"{ Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) }/log/";
		private static readonly string defaultLogFileName = $"{ DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" ) }.log";
		private static LogMode mode = LogMode.Disabled;
		private static string logFolder = string.Empty;
		private static string logFileName = string.Empty;
		private static string logFilePath = string.Empty;


		public static LogMode Mode => mode;


		public static void Disable()
		{
			mode = LogMode.Disabled;
			Console.WriteLine( "Logging is disabled." );
		}

		public static void EnableForConsoleOnly( string logDirectory )
		{
			mode = LogMode.ConsoleOnly;
			Log.SetLogFolder( logDirectory );
			Console.WriteLine( "Console logging is enabled." );
		}

		public static void EnableForFileOnly( string logDirectory )
		{
			mode = LogMode.FileOnly;
			Log.SetLogFolder( logDirectory );
			Console.WriteLine( "File logging is enabled." );
		}

		public static void EnableForAll( string logDirectory )
		{
			mode = LogMode.All;
			Log.SetLogFolder( logDirectory );
			Console.WriteLine( "Console and file logging are enabled." );
		}

		/// <summary>
		/// Write formatted details about an error to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Error( string message = "" )
			=> WriteLineWithTimestamp( $"ERROR: { message }", ConsoleColor.White, ConsoleColor.Red );

		/// <summary>
		/// Write formatted important information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Important( string message = "" )
			=> WriteLineWithTimestamp( $"{ message }", ConsoleColor.Cyan );

		/// <summary>
		/// Write formatted information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Info( string message = "" )
			=> WriteLineWithTimestamp( $"{ message }" );

		public static void SetLogFileName( string fileName )
		{
			logFileName = fileName;
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

		/// <summary>
		/// Write formatted details about a task finishing to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void TaskFinished( string message = "" )
			=> WriteLineWithTimestamp( $"FINISHED: { message }", ConsoleColor.Green );
		
		/// <summary>
		/// Write formatted details about a task starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void TaskStarted( string message = "" )
			=> WriteLineWithTimestamp( $"STARTED: { message }", ConsoleColor.Yellow );

		/// <summary>
		/// Write formatted details about a warning starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Warning( string message = "" )
			=> WriteLineWithTimestamp( $"WARNING: { message }", ConsoleColor.White, ConsoleColor.DarkYellow );

		/// <summary>
		/// Write text directly to log without timestamp.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void Write( string message = "", ConsoleColor consoleTextColor = ConsoleColor.White, ConsoleColor consoleHighlightColor = ConsoleColor.Black )
		{
			if ( mode == LogMode.Disabled || message == "" )
			{
				return;
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
			if ( mode == LogMode.All )
			{
				Console.BackgroundColor = consoleHighlightColor;
				Console.ForegroundColor = consoleTextColor;
				Console.Write( message );
				Console.BackgroundColor = defaultConsoleBackgroundColor;
				Console.ForegroundColor = defaultConsoleForegroundColor;
			}
		}

		/// <summary>
		/// Write text directly to log without timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLine( string message = "", ConsoleColor consoleTextColor = ConsoleColor.White, ConsoleColor consoleHighlightColor = ConsoleColor.Black )
			=> Write( $"{ message }\n", consoleTextColor, consoleHighlightColor );

		/// <summary>
		/// Write text directly to log with timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLineWithTimestamp( string message = "", ConsoleColor consoleTextColor = ConsoleColor.White, ConsoleColor consoleHighlightColor = ConsoleColor.Black )
			=> WriteLine( AddTimestampToMessage( message ), consoleTextColor, consoleHighlightColor );

		/// <summary>
		/// Write text directly to log with timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteWithTimestamp( string message = "", ConsoleColor consoleTextColor = ConsoleColor.White, ConsoleColor consoleHighlightColor = ConsoleColor.Black )
			=> Write( AddTimestampToMessage( message ), consoleTextColor, consoleHighlightColor );


		private static string AddTimestampToMessage( string message = "" )
			=> $"[{ DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) }] { message }";

		private static void SetLogFilePath()
			=> logFilePath = $"{ logFolder }{ logFileName }";
	}
}
