using Petrichor.App.Utilities;
using Petrichor.Common.Info;
using Petrichor.Logging;
using System.Text;


namespace Petrichor.App
{
	static class Program
	{
		static async Task Main( string[] args )
		{
			LogFormatVisualizer.ShowTestMessagesInDebug();

			Console.Title = AppInfo.AppName;
			var startTime = DateTime.Now;
			var startTimeMessage = $"Execution started at {startTime:yyyy-MM-dd:HH:mm:ss.fffffff}.";
			Console.WriteLine( AppInfo.AppNameAndVersion );
			Console.WriteLine( startTimeMessage );

			try
			{
				Log.EnableBuffering();
				Log.Info( AppInfo.AppNameAndVersion );
				Log.Info( startTimeMessage );
				var commandToRun = await CommandLineHandler.ParseArguments( args );
				var optionListStringBuilder = new StringBuilder();
				foreach ( var option in commandToRun.Options )
				{
					_ = optionListStringBuilder.Append( $" {option.Key} {option.Value}" );
				}
				Log.Info( $"Command to run: {commandToRun.Name}{optionListStringBuilder}" );
				RuntimeHandler.Execute( commandToRun );
			}
			catch ( Exception exception )
			{
				Log.Error( $"Error occurred during execution: {exception.Message}" );
				Log.Important( $"If you file a bug report, please include the input and log files to help developers reproduce the issue." );
			}

			var endTime = DateTime.Now;
			var executionTime = ( endTime - startTime ).TotalSeconds;
			var finishTimeMessage = $"Execution finished at {DateTime.Now:yyyy-MM-dd:HH:mm:ss.fffffff} and took {executionTime} seconds.";
			Log.Info( finishTimeMessage );
			Console.WriteLine( finishTimeMessage );
			Console.WriteLine();

			RuntimeHandler.ExitApp();
		}
	}
}
