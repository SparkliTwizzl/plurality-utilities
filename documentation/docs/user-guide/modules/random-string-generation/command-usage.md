---
short_title: Command usage
title: Command usage - Random String Generation - Modules
---

<h1 align="center">Command usage</h1>
<h2 align="center"><a href="./index.html">Random String Generation</a></h2>


---
## Module command

To generate a list of random strings, call Petrichor with the command argument `generateRandomStrings`.

This command supports the following options:

- [Allowed characters](#allowed-characters-option)
- [Auto exit](../../getting-started/command-usage.html#auto-exit-option)
- [Log file](../../getting-started/command-usage.html#log-file-option)
- [Log mode](../../getting-started/command-usage.html#log-mode-option)
- [Output file](../../getting-started/command-usage.html#output-file-option)
- [String count](#string-count-option)
- [String length](#string-length-option)


???+ example

    === "Command line"

        ```powershell
        [install path]> Petrichor.exe generateRandomStrings --allowedCharacters [string] --stringCount [integer] --stringLength [integer] --outputFile [file] --logMode [mode] --logFile [file]
        ```

    === "Petrichor Script"

        ```petrichor
        metadata:
        {
            command: generateRandomStrings
            {
                // input-file: Implict when command is in input file. Not used by module.
                allowed-characters: <string>
                auto-exit:
                string-count: <integer>
                string-length: <integer>
                output-file: <file>
                log-mode: <mode>
                log-file: <file>
            }
        }
        ```


---
## Command options

These options are specific to this module.


---
### Allowed characters option

Command line syntax: `#!powershell --allowedCharacters <string>`

Petrichor Script syntax: `#!ptcr allowed-characters : <string>`

The `allowed characters` option allows you to specify the set of characters that random strings will be generated from.

!!! Tip

    You can control the relative frequency of characters by including multiple instances of characters in the argument.

!!! important "Restrictions"

    OPTIONAL

    In Petrichor Script, must be within [`command`](../../getting-started/petrichor-script.html#command-token) token body.

    If there are spaces in the argument, it must be surrounded by quotes ( `"` ).

???+ example

    === "Command line"

        === "Equal character frequency"

            ```powershell title="All characters will appear the same number of times, on average."
            Petrichor.exe commandName --allowedCharacters "abcd"
            ```

        === "Weighted character frequency"

            ```powershell title="'a' will appear 4 times as often as other characters, on average."
            Petrichor.exe commandName --allowedCharacters "aaaabcd"
            ```

    === "Petrichor Script"

        === "Equal character frequency"

            ```petrichor title="All characters will appear the same number of times, on average."
            metadata:
            {
                // ...
                command: commandName
                {
                    allowed-characters: abcd
                }
            }
            ```

        === "Weighted character frequency"

            ```petrichor title="'a' will appear 4 times as often as other characters, on average."
            metadata:
            {
                // ...
                command: commandName
                {
                    allowed-characters: aaaabcd
                }
            }
            ```


---
### String count option

Command line syntax: `#!powershell --stringCount <integer>`

Petrichor Script syntax: `#!ptcr string-count : <integer>`

The `string count` option allows you to control the number of random strings to be generated.

!!! important "Restrictions"

    OPTIONAL

    In Petrichor Script, must be within [`command`](../../getting-started/petrichor-script.html#command-token) token body.

    Argument must be a positive integer (>= 1).

???+ example

    === "Command line"

        ```powershell
        Petrichor.exe commandName --stringCount 10
        ```

    === "Petrichor Script"

        ```petrichor
        metadata:
        {
            // ...
            command: commandName
            {
                string-count: 10
            }
        }
        ```


---
### String length option

Command line syntax: `#!powershell --stringLength <integer>`

Petrichor Script syntax: `#!ptcr string-length : <integer>`

The `string count` option allows you to control the length of randomly generated strings.

!!! important "Restrictions"

    OPTIONAL

    In Petrichor Script, must be within [`command`](../../getting-started/petrichor-script.html#command-token) token body.

    Argument must be a positive integer (>= 1).

???+ example

    === "Command line"

        ```powershell
        Petrichor.exe commandName --stringLength 10
        ```

    === "Petrichor Script"

        ```petrichor
        metadata:
        {
            // ...
            command: commandName
            {
                string-length: 10
            }
        }
        ```
