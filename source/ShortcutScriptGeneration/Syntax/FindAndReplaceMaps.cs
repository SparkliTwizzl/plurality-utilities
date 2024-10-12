namespace Petrichor.ShortcutScriptGeneration.Syntax
{
	/// <summary>
	/// Contains maps of find-and-replace pairs.
	/// </summary>
	public static class FindAndReplaceMaps
	{
		/// <summary>
		/// Find-and-replace map for keyboard shortcuts to control shortcut scripts.
		/// </summary>
		public static Dictionary<string, string> ScriptControlKeyboardShortcuts => new()
		{
			{ "[windows]", "#" },
			{ "[win]", "#" },
			{ "[alt]", "!" },
			{ "[left-alt]", "<!" },
			{ "[lalt]", "<!" },
			{ "[right-alt]", ">@" },
			{ "[ralt]", ">@" },
			{ "[control]", "^" },
			{ "[ctrl]", "^" },
			{ "[left-control]", "<^" },
			{ "[lctrl]", "<^" },
			{ "[right-control]", ">^" },
			{ "[rctrl]", ">^" },
			{ "[shift]", "+" },
			{ "[left-shift]", "<+" },
			{ "[lshift]", "<+" },
			{ "[right-shift]", ">+" },
			{ "[rshift]", ">+" },
			{ "[combine]", "&" },
			{ "[and]", "&" },
			{ "[alt-graph]", "<^>!" },
			{ "[altgr]", "<^>!" },
			{ "[wildcard]", "*" },
			{ "[asterisk]", "*" },
			{ "[passthrough]", "~" },
			{ "[tilde]", "~" },
			{ "[send]", "$" },
			{ "[dollar]", "$" },
			{ "[tab]", "Tab" },
			{ "[caps-lock]", "CapsLock" },
			{ "[caps]", "CapsLock" },
			{ "[enter]", "Enter" },
			{ "[backspace]", "Backspace" },
			{ "[bksp]", "Backspace" },
			{ "[insert]", "Insert" },
			{ "[ins]", "Insert" },
			{ "[delete]", "Delete" },
			{ "[del]", "Delete" },
			{ "[home]", "Home" },
			{ "[end]", "End" },
			{ "[page-up]", "PageUp" },
			{ "[pgup]", "PageUp" },
			{ "[page-down]", "PageDown" },
			{ "[pgdn]", "PageDown" },
			{ "[open-braacket]", "[" },
			{ "[close-bracket]", "]" },
		};
	}
}
