using System;
using System.IO;
using System.Reflection;


namespace PluralityUtilities.Logging
{
	public static class Logger
	{
		private static StreamWriter _logFile;
		private static readonly string _logFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";


		static Logger()
		{
			var logFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/log/";
			_logFile = new StreamWriter(logFilePath + _logFileName);
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
