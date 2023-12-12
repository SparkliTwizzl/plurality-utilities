using Petrichor.AutoHotkeyScripts.Utilities;
using Petrichor.Common.Utilities;
using Petrichor.Logging;
using Petrichor.Logging.Enums;


namespace Petrichor.App
{
	static class Program
	{
		private static string inputFilePath = string.Empty;
		private static LogMode logMode = LogMode.Disabled;
		private static string outputFilePath = string.Empty;
		private static DateTime startTime;


		static void Main( string[] args )
		{
			startTime = DateTime.Now;
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
			Log.WriteLine( $"PluralityUtilities v{ AppVersion.CurrentVersion }" );
			Log.WriteLine( $"execution started at { startTime.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) }" );
			CreateAutoHotkeyScript();
			Log.WriteLine( $"execution finished at { DateTime.Now.ToString( "yyyy-MM-dd:HH:mm:ss.fffffff" ) } (took { ( DateTime.Now - startTime ).TotalSeconds } seconds)" );
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

				var input = inputParser.ParseInputFile( inputFilePath );
				var macros = scriptGenerator.GenerateMacrosFromInput( input );
				scriptGenerator.GenerateScript( macros, outputFilePath );

				var successMessage = "generating script succeeded";
				Console.WriteLine( successMessage );
				Log.WriteLine( successMessage );
			}
			catch ( Exception ex )
			{
				var errorMessage = $"generating script failed with error: { ex.Message }";
				if ( logMode != LogMode.Verbose )
				{
					Console.WriteLine( errorMessage );
				}
				Log.WriteLine( errorMessage );
			}
		}

		private static void InitLogging()
		{
			switch ( logMode )
			{
				case LogMode.Basic:
					{
						Log.EnableBasic();
						Log.SetLogFolder( ProjectDirectories.LogDirectory );
						Console.WriteLine( "logging is enabled" );
						break;
					}

				case LogMode.Verbose:
					{
						Log.EnableVerbose();
						Log.SetLogFolder( ProjectDirectories.LogDirectory );
						Console.WriteLine( "verbose logging is enabled" );
						break;
					}

				default:
					{
						Console.WriteLine( "logging is disabled" );
						break;
					}
			}
		}

		private static void ParseArgs( string[] args )
		{
			inputFilePath = args[ 0 ];
			outputFilePath = args[ 1 ];
			if ( args.Length > 2 )
			{
				var arg = args[ 2 ];
				if ( string.Compare( arg, "-l" ) == 0 )
				{
					logMode = LogMode.Basic;
				}
				else if ( string.Compare( arg, "-v" ) == 0 )
				{
					logMode = LogMode.Verbose;
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
