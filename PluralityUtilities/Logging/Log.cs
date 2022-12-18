using System;
using System.IO;
using System.Reflection;


namespace PluralityUtilities.Logging
{
	public static class Log
	{
		private static StreamWriter _logFile;
		private static readonly string _logFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";


		static Log()
		{
			var logFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/log/";
			_logFile = new StreamWriter(logFilePath + _logFileName);
		}

		public static void Write(string message)
		{
			_logFile.Write(message);
		}

		public static void WriteLine(string message)
		{
			_logFile.WriteLine();
		}
	}
}
