using Petrichor.App.Utilities;
using Petrichor.Common;
using Petrichor.Logging;


namespace Petrichor.App
{
	static class Program
	{
		private static DateTime startTime;


		static void Main( string[] args )
		{
			startTime = DateTime.Now;
			Console.WriteLine( AppInfo.AppNameAndVersion );
			CommandLineHandler.ParseArguments( args );
			Log.Important( AppInfo.AppNameAndVersion );
			Log.Important( $"execution started at { startTime.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) }" );
			RuntimeHandler.Execute();
			Log.Important( $"execution finished at { DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) } (took { ( DateTime.Now - startTime ).TotalSeconds } seconds)" );
			RuntimeHandler.WaitForUserAndExit();
		}
	}
 }
