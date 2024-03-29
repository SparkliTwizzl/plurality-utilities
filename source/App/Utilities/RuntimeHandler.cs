using Petrichor.App.Syntax;
using Petrichor.Common.Containers;
using Petrichor.Logging;


namespace Petrichor.App.Utilities
{
	public static class RuntimeHandler
	{
		public static void Execute( ModuleCommand command )
		{
			switch ( command.Name )
			{
				case Commands.GenerateShortcutScript:
					ShortcutScriptGenerationHandler.GenerateScript( command );
					break;

				case Common.Syntax.Commands.Some:
					Log.Error( "Unrecognized command." );
					break;

				case "":
					Log.Error( "No command provided." );
					break;

				default:
					break;
			}
		}

		public static void WaitForUserAndExit()
		{
			Console.Write( "Press any key to exit..." );
			_ = Console.ReadKey( true );
			Environment.Exit( 0 );
		}
	}
}
