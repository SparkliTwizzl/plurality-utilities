#if RELEASE
using System.Diagnostics;
using System.IO;
#endif
using System.Reflection;

using PluralityUtilities.Logging;


namespace PluralityUtilities.App
{
	class Program
	{
		static void Main(string[] args)
		{
#if DEBUG
			Log.SetLogFolder(SolutionFolders.SolutionDir + "/run/log/");
#elif RELEASE
			Log.SetLogFolder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + Process.GetCurrentProcess().ProcessName + "_log/");
#endif

			Log.WriteLine("test");
		}
	}
}
