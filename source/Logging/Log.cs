using Petrichor.Logging.Containers;
using Petrichor.Logging.Enums;
using Petrichor.Logging.Styling;
using Petrichor.Logging.Utilities;
using System.IO;


namespace Petrichor.Logging
{
	public static class Log
	{
		private static string LogFilePath { get; set; } = string.Empty;
		private static List<Message> MessageBuffer { get; set; } = new();


		public static LogMode ActiveMode { get; private set; } = LogMode.All;
		public static bool IsLoggingToConsoleEnabled => ActiveMode is LogMode.Test or LogMode.ConsoleOnly or LogMode.All;
		public static bool IsLoggingToFileEnabled => ActiveMode is LogMode.Test or LogMode.FileOnly or LogMode.All;
		public static bool IsBufferingEnabled { get; private set; } = false;
		public static bool IsInTestMode => ActiveMode is LogMode.Test;


		public static void CreateLogFile( string filePath )
		{
			LogFilePath = filePath;
			var directory = Path.GetDirectoryName( filePath ) ?? string.Empty;
			_ = Directory.CreateDirectory( directory );
			File.Create( LogFilePath ).Close();
			Info( $"Created log file \"{LogFilePath}\"." );
		}

		/// <summary>
		/// Write formatted debug information to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		public static void Debug( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Debug, lineNumber );

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
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Error( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Error, lineNumber );

		/// <summary>
		/// Write information about a task finishing to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Finish( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Finish, lineNumber );

		/// <summary>
		/// Write information with custom label and color scheme to log.
		/// </summary>
		/// <param name="label">Label text to apply to message.</param>
		/// <param name="text">Information to write to log.</param>
		/// <param name="format">Custom coloration to apply to message.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Formatted( string text = "", MessageFormat? format = null, int? lineNumber = null )
			=> WriteFormattedMessage( text, format ?? new(), lineNumber );

		/// <summary>
		/// Write important information to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Important( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Important, lineNumber );

		/// <summary>
		/// Write information to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Info( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Info, lineNumber );

		/// <summary>
		/// Write information about a task starting to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Start( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Start, lineNumber );

		/// <summary>
		/// Write a warning message to log.
		/// </summary>
		/// <param name="text">Information to write to log.</param>
		/// <param name="lineNumber">Line number that message relates to.</param>
		public static void Warning( string text = "", int? lineNumber = null )
			=> WriteFormattedMessage( text, StandardMessageFormats.Warning, lineNumber );

		/// <summary>
		/// Write text to log.
		/// </summary>
		/// <param name="text">Text to write to log.</param>
		public static void Write( string text )
			=> WriteOrBuffer( new() { Text = text } );

		/// <summary>
		/// Write message to log.
		/// </summary>
		/// <param name="message">Message to write to log.</param>
		public static void Write( Message message )
			=> WriteOrBuffer( message );

		/// <summary>
		/// Write all buffered messages to log file and clear buffer.
		/// Does not disable log buffering.
		/// </summary>
		public static void WriteBufferToFile()
		{
			if ( LogFilePath == string.Empty )
			{
				ExceptionLogger.LogAndThrow( new FileNotFoundException( "Log file path was not set." ) );
			}
			foreach ( var message in MessageBuffer )
			{
				WriteToFile( message );
			}
			MessageBuffer.Clear();
			Info( "Wrote log buffer to file." );
		}

		/// <summary>
		/// Write a line of text to log.
		/// </summary>
		/// <param name="line">Line of text to write to log.</param>
		public static void WriteLine( string line )
		{
			Write( line );
			Write( "\n" );
		}

		/// <summary>
		/// Write a message to log.
		/// </summary>
		/// <param name="message">Message to write to log.</param>
		public static void WriteLine( Message message )
		{
			Write( message );
			Write( "\n" );
		}

		/// <summary>
		/// Write a timestamped message to log.
		/// </summary>
		/// <param name="message">Message to write to log.</param>
		public static void WriteLineWithTimestamp( Message message )
		{
			Write( FormattedTimestamp() );
			WriteLine( message );
		}

		/// <summary>
		/// Write timestamped message to log.
		/// </summary>
		/// <param name="message">Message to write to log.</param>
		public static void WriteWithTimestamp( Message message )
		{
			Write( FormattedTimestamp() );
			Write( message );
		}


		private static string FormattedTimestamp() => $"[{DateTime.Now:yyyy-MM-dd:HH:mm:ss.fffffff}]";

		private static void WriteFormattedMessage( string text, MessageFormat format, int? lineNumber = null )
		{
			var formatWithLineNumber = format;
			formatWithLineNumber.LineNumber = lineNumber;
			var message = new Message()
			{
				Text = text,
				Format = formatWithLineNumber,
			};
			WriteLineWithTimestamp( message );
		}

		private static void WriteOrBuffer( Message message )
		{
			if ( message.Text == string.Empty )
			{
				return;
			}
			WriteToConsole( message );
			if ( IsBufferingEnabled )
			{
				MessageBuffer.Add( message );
				return;
			}
			WriteToFile( message );
		}

		private static void WriteToConsole( Message message )
		{
			if ( !IsLoggingToConsoleEnabled )
			{
				return;
			}
			Console.ResetColor();
			Console.Write( message.Formatted() );
		}

		private static void WriteToFile( Message message )
		{
			if ( !IsLoggingToFileEnabled || LogFilePath == string.Empty )
			{
				return;
			}
			using var logFile = File.AppendText( LogFilePath );
			logFile.Write( message.FormattedWithoutColor() );
		}
	}
}
