using System.IO;


namespace PluralityUtilities.Logging
{
	public static class Log
	{
		private static string _logFileFolder;
		private static string _logFileName;
		private static string _logFilePath;


		static Log()
		{
		}


		public static void SetLogFileName(string filename)
		{
			_logFileName = filename;
			SetLogFilePath();
		}

		public static void SetLogFolder(string folder)
		{
			_logFileFolder = folder;
			SetLogFilePath();
		}

		public static void Write(string message = "")
		{
			using (StreamWriter logFile = new StreamWriter(_logFilePath))
			{
				logFile.Write(message);
			}
		}

		public static void WriteLine(string message = "")
		{
			using (StreamWriter logFile = new StreamWriter(_logFilePath))
			{
				logFile.WriteLine(message);
			}
		}


		private static void SetLogFilePath()
		{
			_logFilePath = _logFileFolder + _logFileName;
		}
	}
}
