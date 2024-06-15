using Pastel;
using Petrichor.Logging.Enums;


namespace Petrichor.Logging
{
	public static class Log
	{
		private struct FormattedMessage
		{
			public ColorScheme? ColorScheme { get; set; } = null;
			public string Text { get; set; } = string.Empty;


			public FormattedMessage() { }
		}

		public struct ColorScheme
		{
			public string Background { get; set; } = "#000000";
			public string Foreground { get; set; } = "#aaaaaa";


			public ColorScheme() { }


			public readonly string Apply( string line )
			{
				if ( IsInTestMode )
				{
					return line;
				}

				return line.Pastel( Foreground ).PastelBg( Background );
			}
		}


		private static readonly ColorScheme DebugColorScheme = new()
		{
			Foreground = "#40ffff",
			Background = "#004040",
		};
		private static readonly ColorScheme ErrorColorScheme = new()
		{
			Foreground = "#ffffff",
			Background = "#c00000",
		};
		private static readonly ColorScheme FinishColorScheme = new()
		{
			Foreground = "#00c080",
			Background = "#202020",
		};
		private const int FormattedMessagePaddingAmount = 10;
		private static readonly ColorScheme ImportantColorScheme = new()
		{
			Foreground = "#ffffff",
			Background = "#007070",
		};
		private static readonly ColorScheme InfoColorScheme = new()
		{
			Foreground = "#909090",
		};
		private static readonly ColorScheme StartColorScheme = new()
		{
			Foreground = "#d09000",
			Background = "#202020",
		};
		private static readonly ColorScheme WarningColorScheme = new()
		{
			Foreground = "#ffffff",
			Background = "#707000",
		};

		private static string LogFilePath { get; set; } = string.Empty;
		private static List<FormattedMessage> MessageBuffer { get; set; } = new();


		public static LogMode ActiveMode { get; private set; } = LogMode.All;
		public static bool IsLoggingToConsoleDisabled => !IsLoggingToConsoleEnabled;
		public static bool IsLoggingToFileDisabled => !IsLoggingToFileEnabled;
		public static bool IsLoggingToConsoleEnabled => ActiveMode is LogMode.Test or LogMode.ConsoleOnly or LogMode.All;
		public static bool IsLoggingToFileEnabled => ActiveMode is LogMode.Test or LogMode.FileOnly or LogMode.All;
		public static bool IsBufferingEnabled { get; private set; } = false;
		public static bool IsInTestMode => ActiveMode is LogMode.Test;


		public static void CreateLogFile( string filePath )
		{
			LogFilePath = filePath;
			File.Create( LogFilePath ).Close();
			Info( $"Created log file \"{LogFilePath}\"." );
		}

		/// <summary>
		/// Write formatted debug information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		public static void Debug( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "DEBUG", message, lineNumber, DebugColorScheme );

		public static void DisableLogging()
		{
			Info( "Disabled logging." );
			ActiveMode = LogMode.None;
		}

		public static void DisableBuffering()
		{
			IsBufferingEnabled = false;
			Info( "Disabled log buffering." );
		}

		public static void EnableBuffering()
		{
			IsBufferingEnabled = true;
			Info( "Enabled log buffering." );
		}

		public static void EnableLoggingToConsole()
		{
			ActiveMode = LogMode.ConsoleOnly;
			Info( "Enabled logging to console." );
		}

		public static void EnableLoggingToFile()
		{
			ActiveMode = LogMode.FileOnly;
			Info( "Enabled logging to file." );
		}

		public static void EnableAllLogDestinations()
		{
			ActiveMode = LogMode.All;
			Info( "Enabled all log destinations." );
		}

		public static void EnableTestMode()
		{
			ActiveMode = LogMode.Test;
			Info( "Enabled test mode." );
			Info( "Enabled all log destinations." );
			Info( "Disabled log color schemes." );
		}

		/// <summary>
		/// Write an error message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Error( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "ERROR", message, lineNumber, ErrorColorScheme );

		/// <summary>
		/// Write information about a task finishing to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Finish( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "FINISH", message, lineNumber, FinishColorScheme );

		/// <summary>
		/// Write information with custom label and color scheme to log.
		/// </summary>
		/// <param name="label">Label text to apply to message.</param>
		/// <param name="message">Information to write to log.</param>
		/// <param name="colorScheme">Custom coloration to apply to message.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Formatted( string label, string message = "", ColorScheme? colorScheme = null, int? lineNumber = null )
			=> WriteFormattedMessage( label.ToUpper(), message, lineNumber, colorScheme );

		/// <summary>
		/// Write important information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Important( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "IMPORTANT", message, lineNumber, ImportantColorScheme );

		/// <summary>
		/// Write information to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Info( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "INFO", message, lineNumber, InfoColorScheme );

		/// <summary>
		/// Write information about a task starting to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Start( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "START", message, lineNumber, StartColorScheme );

		/// <summary>
		/// Write a warning message to log.
		/// </summary>
		/// <param name="message">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Warning( string message = "", int? lineNumber = null )
			=> WriteFormattedMessage( "WARNING", message, lineNumber, WarningColorScheme );

		/// <summary>
		/// Write text to log.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="colorScheme">Colors to apply to message in console.</param>
		public static void Write( string message = "", ColorScheme? colorScheme = null )
			=> WriteOrBuffer( message, colorScheme );

		/// <summary>
		/// Write all buffered messages to log file and clear buffer.
		/// Does not disable log buffering.
		/// </summary>
		public static void WriteBufferToFile()
		{
			Info( "Wrote log buffer to file." );
			foreach ( var message in MessageBuffer )
			{
				WriteToFile( message.Text );
			}
			MessageBuffer.Clear();
		}

		/// <summary>
		/// Write a line of text to log.
		/// </summary>
		/// <param name="message">Line of text to write to log.</param>
		/// <param name="colorScheme">Colors to apply to message in console.</param>
		public static void WriteLine( string message = "", ColorScheme? colorScheme = null )
		{
			Write( message, colorScheme );
			Write( "\n" );
		}

		/// <summary>
		/// Write a timestamped line of text to log.
		/// </summary>
		/// <param name="message">Line of text to write to log.</param>
		/// <param name="colorScheme">Colors to apply to message in console.</param>
		public static void WriteLineWithTimestamp( string message = "", ColorScheme? colorScheme = null )
			=> WriteLine( AddTimestampToMessage( message ), colorScheme );

		/// <summary>
		/// Write timestamped textto log.
		/// </summary>
		/// <param name="message">Text to write to log.</param>
		/// <param name="colorScheme">Colors to apply to message in console.</param>
		public static void WriteWithTimestamp( string message = "", ColorScheme? colorScheme = null )
			=> Write( AddTimestampToMessage( message ), colorScheme );


		private static string AddTimestampToMessage( string message = "" )
			=> $"[{DateTime.Now:yyyy-MM-dd:HH:mm:ss.fffffff}] {message}";

		private static void WriteFormattedMessage( string label, string message = "", int? lineNumber = null, ColorScheme? colorScheme = null )
		{
			var hasLineNumber = lineNumber is not null;
			var lineNumberString = hasLineNumber ? $"<LINE {lineNumber}> " : string.Empty;
			var formattedLabel = string.Format( "{0," + FormattedMessagePaddingAmount + "}", $"{label}" );
			var formattedMessage = $"{formattedLabel} : {lineNumberString}{message}";
			WriteLineWithTimestamp( formattedMessage, colorScheme );
		}

		private static void WriteOrBuffer( string message = "", ColorScheme? colorScheme = null )
		{
			if ( message == string.Empty )
			{
				return;
			}

			WriteToConsole( message, colorScheme );
			
			if ( !IsBufferingEnabled )
			{
				WriteToFile( message );
				return;
			}

			MessageBuffer.Add( new()
			{
				ColorScheme = colorScheme,
				Text = message,
			} );
		}

		private static void WriteToConsole( string message, ColorScheme? colorScheme = null )
		{
			if ( IsLoggingToConsoleDisabled )
			{
				return;
			}
			var formattedMessage = colorScheme?.Apply( message ) ?? message;
			Console.ResetColor();
			Console.Write( formattedMessage );
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
