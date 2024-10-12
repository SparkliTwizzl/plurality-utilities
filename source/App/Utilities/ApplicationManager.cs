using Petrichor.Common.Containers;
using Petrichor.Logging;


namespace Petrichor.App.Utilities
{
	/// <summary>
	/// Handles the execution of runtime commands and manages application exit behavior.
	/// </summary>
	public static class ApplicationManager
	{
		/// <summary>
		/// Executes the provided <see cref="ModuleCommand"/> by dispatching it to the appropriate module handler.
		/// </summary>
		/// <param name="command">The <see cref="ModuleCommand"/> to be executed.</param>
		public static void HandleModuleCommand(ModuleCommand command)
		{
			switch (command.Name)
			{
				case RandomStringGeneration.Syntax.Commands.ModuleCommand:
					RandomStringGeneration.Utilities.ModuleHandler.ExecuteCommand(command);
					break;

				case ShortcutScriptGeneration.Syntax.Commands.ModuleCommand:
					ShortcutScriptGeneration.Utilities.ModuleHandler.ExecuteCommand(command);
					break;

				case Common.Syntax.Commands.Some:
					Logger.Error("Unrecognized command.");
					break;

				case "":
					Logger.Error("No command provided.");
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Exits the application. If auto-exit is disabled, waits for a key press before exiting.
		/// </summary>
		public static void TerminateApplication()
		{
			if (!Common.Utilities.TerminalOptions.IsAutoExitEnabled)
			{
				Console.Write("Press any key to exit...");
				_ = Console.ReadKey(true);
			}
			Environment.Exit(0);
		}
	}
}
