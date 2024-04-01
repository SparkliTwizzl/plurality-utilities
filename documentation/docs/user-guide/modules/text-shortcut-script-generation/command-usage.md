---
short_title: Command usage
title: Command usage - Text Shortcut Script Generation module
---

<h1 align="center">Command usage</h1>
<h2 align="center"><a href="./index.html">Text Shortcut Script Generation module</a></h2>


---
## generateTextShortcutScript command

To generate a text hotstring shortcut script, call Petrichor with the command argument `generateTextShortcutScript`.

This command supports the following options:

- [Input file](../../getting-started/command-usage.html#input-file-option)
- [Output file](../../getting-started/command-usage.html#output-file-option)
- [Log mode](../../getting-started/command-usage.html#log-mode-option)
- [Log file](../../getting-started/command-usage.html#log-file-option)


**Example:**

Command line:
```powershell
[install path]\Petrichor> Petrichor.exe generateTextShortcutScript --inputFile [file] --outputFile [file] --logMode [mode] --logFile [file]
```

Petrichor Script:
```petrichor
metadata:
{
	command: generateTextShortcutScript
	{
		// input-file: implict
		output-file: [file]
		log-mode: [mode]
		log-file: [file]
	}
}
```
