using Petrichor.App.Utilities;
using Petrichor.Common.Info;
using Petrichor.Logging;


namespace Petrichor.App
{
	static class Program
	{
		static async Task Main( string[] args )
		{
			Console.Title = AppInfo.AppName;
			var startTime = DateTime.Now;
			Console.WriteLine( AppInfo.AppNameAndVersion );
			var startTimeMessage = $"Execution started at {startTime.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )}";
			Console.WriteLine( startTimeMessage );
			var moduleToRun = await CommandLineHandler.ParseArguments( args );
			Log.Info( AppInfo.AppNameAndVersion );
			Log.Info( startTimeMessage );

			try
			{
				RuntimeHandler.Execute( moduleToRun );
			}
			catch ( Exception exception )
			{
				Log.Error( $"Error occurred during execution: {exception.Message}" );
				Log.Important( $"If you file a bug report, please attach the input and log files to help developers reproduce the error." );
			}

			var endTime = DateTime.Now;
			var executionTime = ( endTime - startTime ).TotalSeconds;
			var finishTimeMessage = $"Execution finished at {DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" )} and took {executionTime} seconds";
			Log.Info( finishTimeMessage );
			Console.WriteLine( finishTimeMessage );

			RuntimeHandler.WaitForUserAndExit();
		}
	}
}
