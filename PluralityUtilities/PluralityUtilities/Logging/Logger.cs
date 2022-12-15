using System;
using System.IO;

namespace PluralityUtilities.Logging
{
	public static class Logger
	{
		private static StreamWriter _logFile;
		private static readonly string _logFileName = ".\\PluralityUtilities";
		private static string _logFilePath;


		static Logger()
		{
			_logFilePath = _logFileName + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".log";
			_logFile = new StreamWriter(_logFilePath);
		}

		public static void Log(string message)
		{
			_logFile.Write(message);
		}

		public static void LogLine(string message)
		{
			_logFile.WriteLine();
		}
	}
}
