using System.Reflection;


namespace PluralityUtilities.Logging
{
	public static class Log
	{
		private static readonly string _defaultLogFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/log/";
		private static readonly string _defaultLogFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
		private static bool _isEnabled = false;
		private static string _logFolder = "";
		private static string _logFileName = "";
		private static string _logFilePath = "";


		static Log() { }


		public static void Disable()
		{
			_isEnabled = false;
		}

		public static void Enable()
		{
			_isEnabled = true;
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
			if (_isEnabled)
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
					logFile.Write(DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss - ") + message);
				}
			}
		}

		public static void WriteLine(string message = "")
		{
			Write($"{message}\n");
		}


		private static void SetLogFilePath()
		{
			_logFilePath = _logFolder + _logFileName;
		}
	}
}
