using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace PluralityUtilities.Logging
{
	public static class Log
	{
		private static readonly string _logFileFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + Process.GetCurrentProcess().ProcessName + "_logs/";
		private static readonly string _logFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
		private static readonly string _logFilePath = _logFileFolder + _logFileName;


		static Log()
		{
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
	}
}
