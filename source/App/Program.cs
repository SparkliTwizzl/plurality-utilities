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
			var startMessage = $"Execution started at {startTime.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )}";
			Console.WriteLine( startMessage );
			Log.Important( AppInfo.AppNameAndVersion );
			Log.Important( startMessage );

			RuntimeHandler.Execute();

			var endTime = DateTime.Now;
			var executionTime = ( endTime - startTime ).TotalSeconds;
			var finishMessage = $"Execution finished at {DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )} and took {executionTime} seconds";
			Log.Important( finishMessage );
			Console.WriteLine( finishMessage );
			
			RuntimeHandler.WaitForUserAndExit();
		}
	}
}
