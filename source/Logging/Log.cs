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


		private static readonly ColorScheme DebugColorScheme = new()
		{
			Foreground = ConsoleColor.Cyan,
			Background = ConsoleColor.DarkBlue,
		};
		private static readonly string DefaultLogDirectory = $@"{AppContext.BaseDirectory}\_log";
		private static readonly string DefaultLogFileName = $"{DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" )}.log";
		private static readonly ColorScheme ErrorColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkRed,
		};
		private static readonly ColorScheme FinishColorScheme = new()
		{
			Foreground = ConsoleColor.Green,
			Background = ConsoleColor.Black,
		};
		private const int FormattedMessagePaddingAmount = 10;
		private static readonly ColorScheme ImportantColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkCyan,
		};
		private static readonly ColorScheme InfoColorScheme = new()
		{
			Foreground = ConsoleColor.Gray,
			Background = ConsoleColor.Black,
		};
		private static readonly ColorScheme StartColorScheme = new()
		{
			Foreground = ConsoleColor.Cyan,
			Background = ConsoleColor.Black,
		};
		private static readonly ColorScheme WarningColorScheme = new()
		{
			Foreground = ConsoleColor.White,
			Background = ConsoleColor.DarkMagenta,
		};

		private static string LogDirectory { get; set; } = DefaultLogDirectory;
		private static string LogFileName { get; set; } = DefaultLogFileName;
		private static string LogFilePath { get; set; } = string.Empty;


		public static LogMode ActiveMode { get; private set; } = LogMode.All;
		public static bool IsLoggingToConsoleDisabled => !IsLoggingToConsoleEnabled;
		public static bool IsLoggingToFileDisabled => !IsLoggingToFileEnabled;
		public static bool IsLoggingToConsoleEnabled => ActiveMode is LogMode.ConsoleOnly or LogMode.All;
		public static bool IsLoggingToFileEnabled => ActiveMode is LogMode.FileOnly or LogMode.All;


		public static void CreateLogFile( string file )
		{
			SetLogFilePath( file );
			_ = Directory.CreateDirectory( LogDirectory );
			File.Create( LogFilePath ).Close();
		}

		/// <summary>
		/// Write formatted debug information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Debug( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "DEBUG", message, lineNumber, DebugColorScheme );

		public static void Disable()
		{
			ActiveMode = LogMode.None;
			Console.WriteLine( "Logging is disabled." );
		}

		public static void EnableForConsole()
		{
			ActiveMode = LogMode.ConsoleOnly;
			Console.WriteLine( "Logging to console is enabled." );
		}

		public static void EnableForFile()
		{
			ActiveMode = LogMode.FileOnly;
			Console.WriteLine( "Logging to file is enabled." );
		}

		public static void EnableForAll()
		{
			EnableForConsole();
			EnableForFile();
			ActiveMode = LogMode.All;
		}

		/// <summary>
		/// Write a formatted error message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Error( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "ERROR", message, lineNumber, ErrorColorScheme );

		/// <summary>
		/// Write formatted details about a task finishing to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Finish( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "FINISH", message, lineNumber, FinishColorScheme );

		/// <summary>
		/// Write formatted important information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Important( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "IMPORTANT", message, lineNumber, ImportantColorScheme );

		/// <summary>
		/// Write formatted information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Info( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "INFO", message, lineNumber, InfoColorScheme );

		/// <summary>
		/// Write formatted details about a task starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Start( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "START", message, lineNumber, StartColorScheme );

		/// <summary>
		/// Write a formatted warning message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Warning( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "WARNING", message, lineNumber, WarningColorScheme );

		/// <summary>
		/// Write text to log.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="consoleForegroundColor">Text color to use in console mode.</param>
		/// <param name="consoleBackgroundColor">Background color to use if in console mode.</param>
		public static void Write( string message = "", ColorScheme? colorScheme = null )
		{
			if ( ActiveMode == LogMode.None || message == string.Empty )
			{
				return;
			}

			WriteToConsole( message, colorScheme );
			WriteToFile( message );
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

		private static void SetLogFilePath( string file )
		{
			LogDirectory = Path.GetDirectoryName( file ) ?? string.Empty;
			if ( LogDirectory == string.Empty )
			{
				LogDirectory = DefaultLogDirectory;
			}

			LogFileName = Path.GetFileName( file ) ?? string.Empty;
			if ( LogFileName == string.Empty )
			{
				LogFileName = DefaultLogFileName;
			}

			LogFilePath = Path.Combine( LogDirectory, LogFileName );
			Console.WriteLine( $"Log file will be created at \"{LogFilePath}\"." );
		}

		private static void WriteFormattedMessage( string label, string message = "", int? lineNumber = null, ColorScheme? colorScheme = null )
		{
			var hasLineNumber = lineNumber is not null;
			var lineNumberString = hasLineNumber ? $"<LINE {lineNumber}> " : string.Empty;
			var formattedLabel = string.Format( "{0," + FormattedMessagePaddingAmount + "}", $"{label}" );
			var formattedMessage = $"{formattedLabel} : {lineNumberString}{message}";
			WriteLineWithTimestamp( formattedMessage, colorScheme );
		}

		private static void WriteToConsole( string message, ColorScheme? colorScheme = null )
		{
			if ( IsLoggingToConsoleDisabled )
			{
				return;
			}

			colorScheme?.Assign();
			Console.Write( message );
			Console.ResetColor();
		}

		private static void WriteToFile( string message )
		{
			if ( IsLoggingToFileDisabled || LogFilePath == string.Empty )
			{
				return;
			}

			using var logFile = File.AppendText( LogFilePath );
			logFile.Write( message );
		}
	}
}
