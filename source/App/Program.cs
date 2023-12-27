using Petrichor.App.Utilities;
using Petrichor.Common.Info;
using Petrichor.Logging;


namespace Petrichor.App
{
	static class Program
	{
		static async Task Main( string[] args )
		{
			var startTime = DateTime.Now;
			Console.WriteLine( AppInfo.AppNameAndVersion );
			_ = await CommandLineHandler.ParseArguments( args );
			Log.Important( AppInfo.AppNameAndVersion );
			Log.Important( $"execution started at {startTime.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )}" );
			RuntimeHandler.Execute();
			var endTime = DateTime.Now;
			var executionTime = ( endTime - startTime ).TotalSeconds;
			Log.Important( $"execution finished at {DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )} and took {executionTime} seconds" );
			RuntimeHandler.WaitForUserAndExit();
		}
	}
}
