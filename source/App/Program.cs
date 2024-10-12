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
				Logger.EnableBuffering();
				Logger.Info( AppInfo.AppNameAndVersion );
				Logger.Info( startTimeMessage );
				var commandToRun = await CommandLineManager.HandleCommandLineInput( args );
				var optionListStringBuilder = new StringBuilder();
				foreach ( var option in commandToRun.Options )
				{
					_ = optionListStringBuilder.Append( $" {option.Key} {option.Value}" );
				}
				Logger.Info( $"Command to run: {commandToRun.Name}{optionListStringBuilder}" );
				ApplicationManager.HandleModuleCommand( commandToRun );
			}
			catch ( Exception exception )
			{
				Logger.Error( $"Error occurred during execution: {exception.Message}" );
				Logger.Important( $"If you file a bug report, please include the input and log files to help developers reproduce the issue." );
			}

			var endTime = DateTime.Now;
			var executionTime = ( endTime - startTime ).TotalSeconds;
			var finishTimeMessage = $"Execution finished at {DateTime.Now:yyyy-MM-dd:HH:mm:ss.fffffff} and took {executionTime} seconds.";
			Logger.Info( finishTimeMessage );
			Console.WriteLine( finishTimeMessage );
			Console.WriteLine();

			ApplicationManager.TerminateApplication();
		}
	}
}
