using Petrichor.Logging.Enums;


namespace Petrichor.App.Utilities
{
	public static class CommandLineHandler
	{
		public static void ParseArguments(string[] args)
		{
			if (args.Length < 1)
			{
				PrintHelpText();
				RuntimeHandler.WaitForUserAndExit();
			}
			RuntimeHandler.InputFilePath = args[0];
			RuntimeHandler.OutputFilePath = args[1];
			if (args.Length > 2)
			{
				var arg = args[2];
				if (string.Compare(arg, "-l") == 0)
				{
					RuntimeHandler.ActiveLogMode = LogMode.Basic;
				}
				else if (string.Compare(arg, "-v") == 0)
				{
					RuntimeHandler.ActiveLogMode = LogMode.Verbose;
				}
			}
		}


		private static void PrintHelpText()
		{
			Console.WriteLine("usage:");
			Console.WriteLine("pass input file path as arg0");
			Console.WriteLine("pass output file path as arg1");
			Console.WriteLine("pass icon path as arg1");
			Console.WriteLine("pass \"-l\" as arg3 to enable basic logging ( log file output only )");
			Console.WriteLine("pass \"-v\" as arg3 to enable verbose logging ( console and log file output )");
		}
	}
}
