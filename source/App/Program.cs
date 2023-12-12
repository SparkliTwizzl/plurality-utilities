using Petrichor.AutoHotkeyScripts.Utilities;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;


namespace Petrichor.App
{
	static class Program
	{
		private static string _inputFilePath = string.Empty;
		private static LogMode _logMode = LogMode.Disabled;
		private static string _outputFilePath = string.Empty;
		private static DateTime _startTime;


		static void Main( string[] args )
		{
			_startTime = DateTime.Now;
			Console.WriteLine( $"PluralityUtilities v{ AppVersion.CurrentVersion }" );
			if ( args.Length < 1 )
			{
				Console.WriteLine( "usage:" );
				Console.WriteLine( "pass input file as arg0" );
				Console.WriteLine( "pass output file as arg1" );
				Console.WriteLine( "pass \"-l\" as arg2 to enable basic logging ( log file output only )" );
				Console.WriteLine( "pass \"-v\" as arg2 to enable verbose logging ( console and log file output )" );
				WaitForUserToExit();
				return;
			}
			ParseArgs( args );
			InitLogging();
			Log.WriteLineTimestamped( $"PluralityUtilities v{ AppVersion.CurrentVersion }; execution started at { _startTime }" );
			CreateAutoHotkeyScript();
			Log.WriteLineTimestamped( $"execution finished in { ( DateTime.Now - _startTime ).TotalSeconds } seconds" );
			WaitForUserToExit();
		}


		private static void CreateAutoHotkeyScript()
		{
			try
			{
				var entryParser = new EntryParser();
				var templateParser = new TemplateParser();
				var inputParser = new InputParser( entryParser, templateParser );
				var scriptGenerator = new AutoHotkeyScriptGenerator();

				var input = inputParser.ParseInputFile( _inputFilePath );
				var macros = scriptGenerator.GenerateMacrosFromInput( input );
				scriptGenerator.GenerateScript( macros, _outputFilePath );

				var successMessage = "generating script succeeded";
				Console.WriteLine( successMessage );
				Log.WriteLineTimestamped( successMessage );
			}
			catch ( Exception ex )
			{
				var errorMessage = $"generating script failed with error: { ex.Message }";
				if ( _logMode != LogMode.Verbose )
				{
					Console.WriteLine( errorMessage );
				}
				Log.WriteLineTimestamped( errorMessage );
			}
		}

		private static void InitLogging()
		{
			switch ( _logMode )
			{
				case LogMode.Basic:
					Log.EnableBasic();
					Log.SetLogFolder( ProjectDirectories.LogDirectory );
					Console.WriteLine( "logging is enabled" );
					break;
				case LogMode.Verbose:
					Log.EnableVerbose();
					Log.SetLogFolder( ProjectDirectories.LogDirectory );
					Console.WriteLine( "verbose logging is enabled" );
					break;
				default:
					Console.WriteLine( "logging is disabled" );
					break;
			}
		}

		private static void ParseArgs( string[] args )
		{
			_inputFilePath = args[ 0 ];
			_outputFilePath = args[ 1 ];
			if ( args.Length > 2 )
			{
				var arg = args[ 2 ];
				if ( string.Compare( arg, "-l" ) == 0 )
				{
					_logMode = LogMode.Basic;
				}
				else if ( string.Compare( arg, "-v" ) == 0 )
				{
					_logMode = LogMode.Verbose;
				}
			}
		}

		private static void WaitForUserToExit()
		{
			Console.Write( "press any key to exit" );
			Console.ReadKey( true );
		}
	}
 }
