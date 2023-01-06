using System.Reflection;

using PluralityUtilities.Logging.Enums;


namespace PluralityUtilities.Logging
{
	public static class Log
	{
		private static readonly string _defaultLogFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/log/";
		private static readonly string _defaultLogFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
		private static LogMode _mode = LogMode.Disabled;
		private static string _logFolder = "";
		private static string _logFileName = "";
		private static string _logFilePath = "";


		public static void Disable()
		{
			_mode = LogMode.Disabled;
		}

		public static void EnableBasic()
		{
			_mode = LogMode.Basic;
		}

		public static void EnableVerbose()
		{
			_mode = LogMode.Verbose;
		}

		public static void SetLogFileName(string filename)
		{
			_logFileName = filename;
			SetLogFilePath();
		}

		public static void SetLogFolder(string folder)
		{
			_logFolder = folder;
			var lastChar = folder[folder.Length - 1];
			if (lastChar != '\\' && lastChar != '/')
			{
				_logFolder += '/';
			}
			Directory.CreateDirectory(_logFolder);
			SetLogFilePath();
		}

		public static void Write(string message = "")
		{
			if (_mode != LogMode.Disabled)
			{
				if (_logFolder == "")
				{
					SetLogFolder(_defaultLogFolder);
				}
				if (_logFileName == "")
				{
					SetLogFileName(_defaultLogFileName);
				}
				using (StreamWriter logFile = File.AppendText(_logFilePath))
				{
					logFile.Write(message);
				}
				if (_mode == LogMode.Verbose)
				{
					Console.Write(message);
				}
			}
		}

		public static void WriteTimestamped(string message = "")
		{
			Write($"{DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss")} - {message}");
		}

		public static void WriteLine(string message = "")
		{
			Write($"{message}\n");
		}

		public static void WriteLineTimestamped(string message = "")
		{
			WriteTimestamped($"{message}\n");
		}


		private static void SetLogFilePath()
		{
			_logFilePath = _logFolder + _logFileName;
		}
	}
}
