---
short_title: Command usage
title: Command usage - Text Shortcut Script Generation - Modules
---

<h1 align="center">Command usage</h1>
<h2 align="center"><a href="./index.html">Text Shortcut Script Generation</a></h2>


---
## Module command

To generate a text hotstring shortcut script, call Petrichor with the command argument `generateTextShortcutScript`.

This command supports the following options:

- [Input file](../../getting-started/command-usage.html#input-file-option)
- [Log file](../../getting-started/command-usage.html#log-file-option)
- [Log mode](../../getting-started/command-usage.html#log-mode-option)
- [Output file](../../getting-started/command-usage.html#output-file-option)


???+ example

    === "Command line"

        ```powershell
        [install path]> Petrichor.exe generateTextShortcutScript --inputFile [file] --outputFile [file] --logMode [mode] --logFile [file]
        ```

    === "Petrichor Script"

        ```petrichor
        metadata:
        {
            command: generateTextShortcutScript
            {
                // input-file: Implict when command is in input file.
                output-file: <file>
                log-mode: <mode>
                log-file: <file>
            }
        }
        ```
