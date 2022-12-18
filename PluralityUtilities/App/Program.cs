using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using PluralityUtilities.Logging;


namespace PluralityUtilities.App
{
	class Program
	{
		static void Main(string[] args)
		{
#if DEBUG
			Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Log.SetLogFolder("" + "/" + Process.GetCurrentProcess().ProcessName + "_logs/");
#elif RELEASE
			_logFileFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + Process.GetCurrentProcess().ProcessName + "_logs/";
#endif
			Log.SetLogFileName(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log");

			Log.WriteLine("test");
		}
	}
}
