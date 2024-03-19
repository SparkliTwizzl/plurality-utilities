using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		public struct ColorScheme
		{
			public ConsoleColor Background { get; set; } = ConsoleColor.Black;
			public ConsoleColor Foreground { get; set; } = ConsoleColor.White;


			public ColorScheme() { }


			public readonly void Assign()
			{
				Console.BackgroundColor = Background;
				Console.ForegroundColor = Foreground;
			}
		}


		private static ColorScheme DebugColorScheme = new()
		{
			Foreground = ConsoleColor.Cyan,
			Background = ConsoleColor.DarkBlue,
		};
		private static ColorScheme DefaultColorScheme = new();
		private static readonly string DefaultLogDirectory = $@"{AppContext.BaseDirectory}\log";
		private static readonly string DefaultLogFileName = $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";
		private static ColorScheme ErrorColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkRed,
		};
		private static ColorScheme FinishColorScheme = new()
		{
			Foreground = ConsoleColor.Green,
			Background = ConsoleColor.Black,
		};
		private const int FormattedMessagePaddingAmount = 8;
		private static ColorScheme ImportantColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkCyan,
		};
		private static ColorScheme InfoColorScheme = new()
		{
			Foreground = ConsoleColor.Gray,
			Background = ConsoleColor.Black,
		};
		private static ColorScheme StartColorScheme = new()
		{
			Foreground = ConsoleColor.Cyan,
			Background = ConsoleColor.Black,
		};
		private static ColorScheme WarningColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkMagenta,
		};

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
		/// Write formatted debug information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Debug( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "DEBUG", message, lineNumber, DebugColorScheme );

		/// <summary>
		/// Write a formatted error message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Error( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "ERROR", message, lineNumber, ErrorColorScheme );

		/// <summary>
		/// Write formatted important information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Important( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "IMPORTANT", message, lineNumber, ImportantColorScheme );

		/// <summary>
		/// Write formatted information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Info( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "INFO", message, lineNumber, InfoColorScheme );

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
		public static void Finish( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "FINISH", message, lineNumber, FinishColorScheme );

		/// <summary>
		/// Write formatted details about a task starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Start( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "START", message, lineNumber, StartColorScheme );

		/// <summary>
		/// Write a formatted warning message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Warning( string message = "", int? lineNumber = null )
			=> WriteFormattedInformation( "WARNING", message, lineNumber, WarningColorScheme );

		/// <summary>
		/// Write text to log.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleForegroundColor">Text color to use in console mode.</param>
		/// <param name="consoleBackgroundColor">Background color to use if in console mode.</param>
		public static void Write( string message = "", ColorScheme? colorScheme = null )
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
				colorScheme?.Assign();
				Console.Write( message );
				DefaultColorScheme.Assign();
			}
			if ( IsLoggingToFileEnabled )
			{
				using var logFile = File.AppendText( LogFilePath );
				logFile.Write( message );
			}
		}

		/// <summary>
		/// Write a line of text to log.
		/// </summary>
		/// <param name="message">Line of text to write to log.</param>
		/// <param name="consoleForegroundColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleBackgroundColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLine( string message = "", ColorScheme? colorScheme = null )
			=> Write( $"{message}\n", colorScheme );

		/// <summary>
		/// Write a timestamped line of text to log.
		/// </summary>
		/// <param name="message">Line of text to write to log.</param>
		/// <param name="consoleForegroundColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleBackgroundColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteLineWithTimestamp( string message = "", ColorScheme? colorScheme = null )
			=> WriteLine( AddTimestampToMessage( message ), colorScheme );

		/// <summary>
		/// Write timestamped textto log.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleForegroundColor">Text color to use if in verbose mode.</param>
		/// <param name="consoleBackgroundColor">Text highlight color to use if in verbose mode.</param>
		public static void WriteWithTimestamp( string message = "", ColorScheme? colorScheme = null )
			=> Write( AddTimestampToMessage( message ), colorScheme );


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

		private static void WriteFormattedInformation( string label, string message = "", int? lineNumber = null, ColorScheme? colorScheme = null )
		{
			var hasLineNumber = lineNumber is not null;
			var lineNumberString = hasLineNumber ? $" <LINE {lineNumber}>" : string.Empty;
			WriteLineWithTimestamp( $"{string.Format( "{0," + FormattedMessagePaddingAmount + "}", label )}{lineNumberString} : {message}", colorScheme );
		}
	}
}
