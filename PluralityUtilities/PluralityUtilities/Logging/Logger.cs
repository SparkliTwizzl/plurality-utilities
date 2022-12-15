using System;
using System.IO;

namespace PluralityUtilities.Logging
{
	public static class Logger
	{
		private static StreamWriter _logFile;
		private static readonly string _logFileName = ".\\";
		private static string _logFilePath;


		static Logger()
		{
			_logFilePath = _logFileName + DateTime.Now + ".log";
			_logFile = new StreamWriter(_logFilePath);
		}

		public static void Log<T>(T message)
		{
			_logFile.Write(message.ToString());
		}

		public static void LogLine<T>(T message)
		{
			_logFile.WriteLine(message.ToString());
		}
	}
}
