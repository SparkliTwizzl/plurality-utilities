using Petrichor.Logging.Containers;
using Petrichor.Logging.Enums;
using Petrichor.Logging.Styling;
using Petrichor.Logging.Utilities;


namespace Petrichor.Logging
{
	/// <summary>
	/// Provides logging functionality for different log modes and destinations.
	/// </summary>
	public static class Logger
	{
		private static string LogFilePath { get; set; } = string.Empty;
		private static List<Message> LogMessageBuffer { get; set; } = new();


		/// <summary>
		/// Gets the current logging mode.
		/// </summary>
		public static LogMode CurrentLogMode { get; private set; } = LogMode.All;

		/// <summary>
		/// Gets a value indicating whether logging to console is enabled.
		/// </summary>
		public static bool IsConsoleLoggingEnabled => CurrentLogMode is LogMode.Test or LogMode.ConsoleOnly or LogMode.All;

		/// <summary>
		/// Gets a value indicating whether logging to file is enabled.
		/// </summary>
		public static bool IsFileLoggingEnabled => CurrentLogMode is LogMode.Test or LogMode.FileOnly or LogMode.All;

		/// <summary>
		/// Gets a value indicating whether log buffering is enabled.
		/// </summary>
		public static bool IsBufferingEnabled { get; private set; } = false;

		/// <summary>
		/// Gets a value indicating whether the logger is in test mode.
		/// </summary>
		public static bool IsInTestMode => CurrentLogMode is LogMode.Test;

		/// <summary>
		/// Creates a new log file at the specified path.
		/// </summary>
		/// <param name="filePath">The path to the log file.</param>
		public static void CreateLogFile(string filePath)
		{
			LogFilePath = filePath;
			var directory = Path.GetDirectoryName(filePath) ?? string.Empty;
			_ = Directory.CreateDirectory(directory);
			File.Create(LogFilePath).Close();
			Info($"Created log file \"{LogFilePath}\".");
		}

		/// <summary>
		/// Writes a debug message to the log.
		/// </summary>
		/// <param name="text">The debug message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Debug(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Debug, lineNumber);

		/// <summary>
		/// Disables logging.
		/// </summary>
		public static void DisableLogging()
		{
			Info("Disabled logging.");
			CurrentLogMode = LogMode.None;
		}

		/// <summary>
		/// Disables log buffering.
		/// </summary>
		public static void DisableBuffering()
		{
			IsBufferingEnabled = false;
			Info("Disabled log buffering.");
		}

		/// <summary>
		/// Enables log buffering.
		/// </summary>
		public static void EnableBuffering()
		{
			IsBufferingEnabled = true;
			Info("Enabled log buffering.");
		}

		/// <summary>
		/// Enables logging to the console.
		/// </summary>
		public static void EnableConsoleLogging()
		{
			CurrentLogMode = LogMode.ConsoleOnly;
			Info("Enabled logging to console.");
		}

		/// <summary>
		/// Enables logging to a file.
		/// </summary>
		public static void EnableFileLogging()
		{
			CurrentLogMode = LogMode.FileOnly;
			Info("Enabled logging to file.");
		}

		/// <summary>
		/// Enables logging to all destinations.
		/// </summary>
		public static void EnableAllLogDestinations()
		{
			CurrentLogMode = LogMode.All;
			Info("Enabled all log destinations.");
		}

		/// <summary>
		/// Enables test mode for logging.
		/// </summary>
		public static void EnableTestMode()
		{
			CurrentLogMode = LogMode.Test;
			Info("Enabled test mode.");
			Info("Enabled all log destinations.");
			Info("Disabled log color schemes.");
		}

		/// <summary>
		/// Writes an error message to the log.
		/// </summary>
		/// <param name="text">The error message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Error(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Error, lineNumber);

		/// <summary>
		/// Writes a task finish message to the log.
		/// </summary>
		/// <param name="text">The finish message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Finish(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Finish, lineNumber);

		/// <summary>
		/// Writes a formatted message with a custom label and color scheme to the log.
		/// </summary>
		/// <param name="text">The message text.</param>
		/// <param name="format">Optional custom message format.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Formatted(string text = "", MessageFormat? format = null, int? lineNumber = null)
			=> WriteFormattedMessage(text, format ?? new(), lineNumber);

		/// <summary>
		/// Writes an important message to the log.
		/// </summary>
		/// <param name="text">The important message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Important(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Important, lineNumber);

		/// <summary>
		/// Writes an informational message to the log.
		/// </summary>
		/// <param name="text">The informational message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Info(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Info, lineNumber);

		/// <summary>
		/// Writes a task start message to the log.
		/// </summary>
		/// <param name="text">The start message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Start(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Start, lineNumber);

		/// <summary>
		/// Writes a warning message to the log.
		/// </summary>
		/// <param name="text">The warning message text.</param>
		/// <param name="lineNumber">Optional line number associated with the message.</param>
		public static void Warning(string text = "", int? lineNumber = null)
			=> WriteFormattedMessage(text, StandardMessageFormats.Warning, lineNumber);

		/// <summary>
		/// Writes an inline text string to the log.
		/// </summary>
		/// <param name="text">The text string to write.</param>
		public static void Write(string text)
			=> WriteOrBuffer(new() { Text = text });

		/// <summary>
		/// Writes an inline message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void Write(Message message)
			=> WriteOrBuffer(message);

		/// <summary>
		/// Writes all buffered messages to the log file and clears the buffer.
		/// Does not disable log buffering.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown when the log file path is not set.</exception>
		public static void WriteBufferToFile()
		{
			if (LogFilePath == string.Empty)
			{
				ExceptionLogger.LogAndThrow(new FileNotFoundException("Log file path was not set."));
			}
			foreach (var message in LogMessageBuffer)
			{
				WriteToFile(message);
			}
			LogMessageBuffer.Clear();
			Info("Wrote log buffer to file.");
		}

		/// <summary>
		/// Writes a text string to the log.
		/// </summary>
		/// <param name="text">The text string to write.</param>
		public static void WriteLine(string text)
		{
			Write(text);
			Write("\n");
		}

		/// <summary>
		/// Writes a message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void WriteLine(Message message)
		{
			Write(message);
			Write("\n");
		}

		/// <summary>
		/// Writes a timestamped message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void WriteLineWithTimestamp(Message message)
		{
			Write(FormattedTimestamp());
			WriteLine(message);
		}

		/// <summary>
		/// Writes an inline timestamped message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void WriteWithTimestamp(Message message)
		{
			Write(FormattedTimestamp());
			Write(message);
		}


		private static string FormattedTimestamp() => $"[{DateTime.Now:yyyy-MM-dd:HH:mm:ss.fffffff}]";

		private static void WriteFormattedMessage(string text, MessageFormat format, int? lineNumber = null)
		{
			var formatWithLineNumber = format;
			formatWithLineNumber.LineNumber = lineNumber;
			var message = new Message()
			{
				Text = text,
				Format = formatWithLineNumber,
			};
			WriteLineWithTimestamp(message);
		}

		private static void WriteOrBuffer(Message message)
		{
			if (message.Text == string.Empty)
			{
				return;
			}
			WriteToConsole(message);
			if (IsBufferingEnabled)
			{
				LogMessageBuffer.Add(message);
				return;
			}
			WriteToFile(message);
		}

		private static void WriteToConsole(Message message)
		{
			if (!IsConsoleLoggingEnabled)
			{
				return;
			}
			Console.ResetColor();
			Console.Write(message.GetFormattedMessage());
		}

		private static void WriteToFile(Message message)
		{
			if (!IsFileLoggingEnabled || LogFilePath == string.Empty)
			{
				return;
			}
			using var logFile = File.AppendText(LogFilePath);
			logFile.Write(message.GetFormattedMessageWithoutColor());
		}
	}
}
