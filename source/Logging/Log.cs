using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		private static LogMode activeMode = LogMode.None;
		private const ConsoleColor defaultConsoleBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor defaultConsoleForegroundColor = ConsoleColor.White;
		private static readonly string defaultLogDirectory = $"{ AppContext.BaseDirectory }/log/";
		private static readonly string defaultLogFileName = $"{ DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" ) }.log";
		private static string logDirectory = string.Empty;
		private static string logFileName = string.Empty;
		private static string logFilePath = string.Empty;


		public static LogMode ActiveMode => activeMode;
		public static bool IsLoggingToConsoleDisabled => !IsLoggingToConsoleEnabled;
		public static bool IsLoggingToFileDisabled => !IsLoggingToFileEnabled;
		public static bool IsLoggingToConsoleEnabled => ActiveMode == LogMode.ConsoleOnly || ActiveMode == LogMode.All;
		public static bool IsLoggingToFileEnabled => ActiveMode == LogMode.FileOnly || ActiveMode == LogMode.All;


		public static void Disable()
		{
			activeMode = LogMode.None;
			Console.WriteLine( "Logging is disabled." );
		}

		public static void EnableForConsoleOnly( string logDirectory )
		{
			activeMode = LogMode.ConsoleOnly;
			Console.WriteLine( "Console logging is enabled." );
		}

		public static void EnableForFileOnly( string logDirectory )
		{
			activeMode = LogMode.FileOnly;
			Console.WriteLine( "File logging is enabled." );
			SetLogDirectory( logDirectory );
		}

		public static void EnableForAll( string logDirectory )
		{
			activeMode = LogMode.All;
			Console.WriteLine( "Console and file logging are enabled." );
			SetLogDirectory( logDirectory );
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

		/// <summary>
		/// Set log file directory and/or name.
		/// </summary>
		/// <param name="file">File name and/or directory to generate log file at.</param>
		public static void SetLogFile( string file )
		{
			string? directory = Path.GetDirectoryName( file );
			logDirectory = ( directory != null ) ? ( directory + "\\" ) : string.Empty;
			string? fileName = Path.GetFileName( file );
			logFileName = ( fileName != null ) ? fileName : string.Empty;
			SetLogFilePath();
		}
		
		public static void SetLogFileName( string fileName )
		{
			logFileName = fileName;
			SetLogFilePath();
		}

		public static void SetLogDirectory( string directory )
		{
			logDirectory = AddTrailingSlashToDirectoryPath( directory );
			Directory.CreateDirectory( logDirectory );
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
			if ( activeMode == LogMode.None || message == "" )
			{
				return;
			}

			if ( logDirectory == "" )
			{
				SetLogDirectory( defaultLogDirectory );
			}
			if ( logFileName == "" )
			{
				SetLogFileName( defaultLogFileName );
			}

			if ( IsLoggingToConsoleEnabled )
			{
				Console.BackgroundColor = consoleHighlightColor;
				Console.ForegroundColor = consoleTextColor;
				Console.Write( message );
				Console.BackgroundColor = defaultConsoleBackgroundColor;
				Console.ForegroundColor = defaultConsoleForegroundColor;
			}
			if ( IsLoggingToFileEnabled )
			{
				using ( StreamWriter logFile = File.AppendText( logFilePath ) )
				{
					logFile.Write( message );
				}
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

		private static string AddTrailingSlashToDirectoryPath( string directory )
		{
			var lastChar = directory[ directory.Length - 1];
			if (lastChar != '\\' && lastChar != '/')
			{
				directory += '/';
			}
			return directory;
		}

		private static void SetLogFilePath()
		{
			logFilePath = $"{ logDirectory }{ logFileName }";
			if ( logFileName.CompareTo( "" ) != 0 )
			{
				Console.WriteLine( $"Log file will be created at \"{ logFilePath }\"" );
			}
		}
	}
}
