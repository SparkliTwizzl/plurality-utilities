using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities
{
	/// <summary>
	/// Defines a processor for plaintext and templated shortcuts.
	/// </summary>
	public interface IShortcutProcessor
	{
		/// <summary>
		/// Generates shortcuts from shortcut templates and entries.
		/// Adds generated shortcuts to the existing shortcut list.
		/// </summary>
		/// <param name="input">The input data to modify.</param>
		/// <returns>The modified <see cref="ShortcutScriptInput"/> instance.</returns>
		ShortcutScriptInput GenerateTemplatedShortcuts(ShortcutScriptInput input);

		/// <summary>
		/// Sanitizes plaintext shortcuts.
		/// Overwrites the existing shortcut list.
		/// </summary>
		/// <param name="input">The input data to modify.</param>
		/// <returns>The modified <see cref="ShortcutScriptInput"/> instance.</returns>
		ShortcutScriptInput SanitizePlaintextShortcuts(ShortcutScriptInput input);
	}
}
