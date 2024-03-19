using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		private const ConsoleColor ConsoleDefaultBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor ConsoleDefaultForegroundColor = ConsoleColor.White;
		private const ConsoleColor ConsoleErrorBackgroundColor = ConsoleColor.DarkRed;
		private const ConsoleColor ConsoleErrorForegroundColor = ConsoleColor.White;
		private const ConsoleColor ConsoleImportantBackgroundColor = ConsoleColor.DarkGreen;
		private const ConsoleColor ConsoleImportantForegroundColor = ConsoleColor.White;
		private const ConsoleColor ConsoleInfoBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor ConsoleInfoForegroundColor = ConsoleColor.Gray;
		private const ConsoleColor ConsoleTaskFinishBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor ConsoleTaskFinishForegroundColor = ConsoleColor.Green;
		private const ConsoleColor ConsoleTaskStartBackgroundColor = ConsoleColor.Black;
		private const ConsoleColor ConsoleTaskStartForegroundColor = ConsoleColor.Cyan;
		private const ConsoleColor ConsoleWarningBackgroundColor = ConsoleColor.DarkYellow;
		private const ConsoleColor ConsoleWarningForegroundColor = ConsoleColor.White;
		private static readonly string DefaultLogDirectory = $@"{AppContext.BaseDirectory}\log";
		private static readonly string DefaultLogFileName = $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";

		private static string LogDirectory { get; set; } = string.Empty;
		private static string LogFileName { get; set; } = string.Empty;
		private static string LogFilePath { get; set; } = string.Empty;


		public static LogMode ActiveMode { get; private set; } = LogMode.None;
		public static bool IsLoggingToConsoleDisabled => !IsLoggingToConsoleEnabled;
		public static bool IsLoggingToFileDisabled => !IsLoggingToFileEnabled;
		public static bool IsLoggingToConsoleEnabled => ActiveMode is LogMode.ConsoleOnly or LogMode.All;
		public static bool IsLoggingToFileEnabled => ActiveMode is LogMode.FileOnly or LogMode.All;


		public static void Disable()
		{
			ActiveMode = LogMode.None;
			Console.WriteLine( "Logging is disabled" );
		}

		public static void EnableForConsoleOnly()
		{
			ActiveMode = LogMode.ConsoleOnly;
			Console.WriteLine( "Console logging is enabled" );
		}

		public static void EnableForFileOnly( string logDirectory )
		{
			ActiveMode = LogMode.FileOnly;
			Console.WriteLine( "File logging is enabled" );
			SetLogDirectory( logDirectory );
		}

		public static void EnableForAll( string logDirectory )
		{
			ActiveMode = LogMode.All;
			Console.WriteLine( "Console and file logging are enabled" );
			SetLogDirectory( logDirectory );
		}

		/// <summary>
		/// Write formatted details about an error to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Error( string message = "" )
			=> WriteLineWithTimestamp( $"    ERROR : {message}", ConsoleErrorForegroundColor, ConsoleErrorBackgroundColor );

		/// <summary>
		/// Write formatted important information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Important( string message = "" )
			=> WriteLineWithTimestamp( $"IMPORTANT : {message}", ConsoleImportantForegroundColor, ConsoleImportantBackgroundColor );

		/// <summary>
		/// Write formatted information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Info( string message = "" )
			=> WriteLineWithTimestamp( $"     INFO : {message}", ConsoleInfoForegroundColor, ConsoleInfoBackgroundColor );

		/// <summary>
		/// Set log file directory and/or name.
		/// </summary>
		/// <param name="file">File name and/or directory to generate log file at.</param>
		public static void SetLogFile( string file )
		{
			var directory = Path.GetDirectoryName( file );
			if ( directory is not null )
			{
				SetLogDirectory( directory );
			}
			var fileName = Path.GetFileName( file );
			if ( fileName is not null )
			{
				SetLogFileName( fileName );
			}
			SetLogFilePath();
		}

		public static void SetLogFileName( string fileName )
		{
			LogFileName = fileName;
			SetLogFilePath();
		}

		public static void SetLogDirectory( string directory )
		{
			LogDirectory = AddTrailingSlashToDirectoryPath( directory );
			_ = Directory.CreateDirectory( LogDirectory );
			SetLogFilePath();
		}

		/// <summary>
		/// Write formatted details about a task finishing to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void TaskFinish( string message = "" )
			=> WriteLineWithTimestamp( $"   FINISH : {message}", ConsoleTaskFinishForegroundColor, ConsoleTaskFinishBackgroundColor );

		/// <summary>
		/// Write formatted details about a task starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void TaskStart( string message = "" )
			=> WriteLineWithTimestamp( $"    START : {message}", ConsoleTaskStartForegroundColor, ConsoleTaskStartBackgroundColor );

		/// <summary>
		/// Write formatted details about a warning starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Warning( string message = "" )
			=> WriteLineWithTimestamp( $"  WARNING : {message}", ConsoleWarningForegroundColor, ConsoleWarningBackgroundColor );

		/// <summary>
		/// Write text directly to log without timestamp.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void Write( string message = "", ConsoleColor consoleTextColor = ConsoleDefaultForegroundColor, ConsoleColor consoleHighlightColor = ConsoleDefaultBackgroundColor )
		{
			if ( ActiveMode == LogMode.None || message == "" )
			{
				return;
			}

			if ( LogDirectory == "" )
			{
				SetLogDirectory( DefaultLogDirectory );
			}
			if ( LogFileName == "" )
			{
				SetLogFileName( DefaultLogFileName );
			}

			if ( IsLoggingToConsoleEnabled )
			{
				Console.BackgroundColor = consoleHighlightColor;
				Console.ForegroundColor = consoleTextColor;
				Console.Write( message );
				Console.BackgroundColor = ConsoleDefaultBackgroundColor;
				Console.ForegroundColor = ConsoleDefaultForegroundColor;
			}
			if ( IsLoggingToFileEnabled )
			{
				using var logFile = File.AppendText( LogFilePath );
				logFile.Write( message );
			}
		}

		/// <summary>
		/// Write text directly to log without timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLine( string message = "", ConsoleColor consoleTextColor = ConsoleDefaultForegroundColor, ConsoleColor consoleHighlightColor = ConsoleDefaultBackgroundColor )
			=> Write( $"{message}\n", consoleTextColor, consoleHighlightColor );

		/// <summary>
		/// Write text directly to log with timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLineWithTimestamp( string message = "", ConsoleColor consoleTextColor = ConsoleDefaultForegroundColor, ConsoleColor consoleHighlightColor = ConsoleDefaultBackgroundColor )
			=> WriteLine( AddTimestampToMessage( message ), consoleTextColor, consoleHighlightColor );

		/// <summary>
		/// Write text directly to log with timestamp, followed by a newline.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleTextColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleHighlightColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteWithTimestamp( string message = "", ConsoleColor consoleTextColor = ConsoleDefaultForegroundColor, ConsoleColor consoleHighlightColor = ConsoleDefaultBackgroundColor )
			=> Write( AddTimestampToMessage( message ), consoleTextColor, consoleHighlightColor );


		private static string AddTimestampToMessage( string message = "" )
			=> $"[{DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )}] {message}";

		private static string AddTrailingSlashToDirectoryPath( string directory )
		{
			var lastChar = directory[ ^1 ];
			if ( lastChar is not '\\' and not '/' )
			{
				directory += '\\';
			}
			return directory;
		}

		private static void SetLogFilePath()
		{
			LogFilePath = $@"{LogDirectory}{LogFileName}";
			if ( LogFileName.CompareTo( "" ) != 0 )
			{
				Console.WriteLine( $"Log file will be created at \"{LogFilePath}\"" );
			}
		}
	}
}
