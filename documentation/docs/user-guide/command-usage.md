<h1 align="center">Command usage</h1>


---
# Commands

Petrichor modules each have a corresponding command to trigger them.

These can be given as command line arguments, or they can be [put into your input file](petrichor-script.md/#command-token) and Petrichor will attempt to read ane execute them automatically.


---
## Default command

You can run Petrichor without passing it a command if you put the command in your input file and pass the input file as an argument when you run it.

You can also just drag-and-drop the input file onto the executable ( `.exe` file ).

If no arguments are provided, you will be prompted to use the `--help` option to see usage and available commands.


---
## Command options

Commands can have their behavior modified with options.

Some commands have unique options.


---
### Input file option

(OPTIONAL)

Command line syntax: `--inputFile [argument]`

Petrichor Script syntax: `input-file : [argument]`

The `input file` option allows you to specify the input file directory and/or name.

If there are spaces in the argument, it must be surrounded by quotes ( `"` ).

You must include the file extension if you provide a file name.

[Relative file paths](#relative-file-paths) can be used.

If you only provide the directory, `input.petrichor` will be used as the file name.

If you only provide the file name, `[install path]\Petrichor\` will be used as the directory.

**Example (File name only, default directory):**
```txt
Command line:      Petrichor.exe commandName --inputFile "inputFile.txt"
Petrichor Script:  input-file : "inputFile.txt"
```
Petrichor will look for `[install path]\Petrichor\inputFile.txt`.

**Example (Directory only, default file name):**
```txt
Command line:      Petrichor.exe commandName --inputFile "[path]\"
Petrichor Script:  input-file : "[path]\"
```
Petrichor will look for `[path]\input.petrichor`.

**Example (Full file path):**
```txt
Command line:      Petrichor.exe commandName --inputFile "[path]\inputFile.txt"
Petrichor Script:  input-file : "[path]\inputFile.txt"
```
Petrichor will look for `[path]\inputFile.txt`.


---
### Output file option

(OPTIONAL)

Command line syntax: `--outputFile [argument]`

Petrichor script syntax: `output-file : [argument]`

The `output file` option the output file directory and/or name for commands which generate files.

If there are spaces in the argument, it must be surrounded by quotes ( `"` ).

A file extension may or may not be required. In some cases it may be overridden by a module.

[Relative file paths](#relative-file-paths) can be used.

If you only provide the directory, `output.[extension]` will be used as the file name.

If you only provide the file name, `[install path]\Petrichor\_output\` will be used as the directory.

**Example (File name only, default directory):**
```txt
Command line:      Petrichor.exe commandName --outputFile "outputFile"
Petrichor Script:  output-file : "outputFile"
```
Petrichor will generate the file `[install path]\Petrichor\_output\outputFile.[extension]`.

**Example (Directory only, default file name):**
```txt
Command line:      Petrichor.exe commandName --outputFile "[path]\output\"
Petrichor Script:  output-file : "[path]\output\"
```
Petrichor will generate the file `[path]\output\output.[extension]`.

**Example (Full file path):**
```txt
Command line:      Petrichor.exe commandName --outputFile "[path]\outputFile"
Petrichor Script:  output-file : "[path]\outputFile"
```
Petrichor will generate the file `[path]\outputFile.[extension]`.

---
### Log mode option

(OPTIONAL)

Command line syntax: `--logMode [argument]`

Petrichor Script syntax: `log-mode : [argument]`

The `log mode` option is used to control where logs are sent.

Allowed values:

- `all` (DEFAULT) - Send logs to all output locations.
- `fileOnly` - Send logs only to log file.
- `consoleOnly` - Send logs only to console output.
- `none` - Disable logging.

**Example:**
```txt
Command line:      Petrichor.exe comandName --logMode [all | fileOnly | consoleOnly | none]
Petrichor Script:  log-file : [all | fileOnly | consoleOnly | none]
```

---
### Log file option

(OPTIONAL)

Command line syntax: `--logFile [argument]`

Petrichor Script syntax: `log-file : [argument]`

The `log file` option is used to specify the file name and/or directory to generate log files at.

If there are spaces in the argument, it must be surrounded by quotes ( `"` ).

A file extension is not required and will be overridden if specified.

**NOTE:** Log file will only be created if logging to file is enabled.

**Example (File name only, default directory):**
```txt
Command line:      Petrichor.exe comandName --logFile "logFile.txt"
Petrichor Script:  log-file : "logFile.txt"
```
Petrichor will generate the file `[install path]\_log\logFile.txt`.

**Example (Directory only, default file name):**
```txt
Command line:      Petrichor.exe commandName --logFile "[path]/"
Petrichor Script:  log-file : [path]/"
```
Petrichor will generate the file `[path]\[default log file name].log`.

**Example: (Full file path):**
```txt
Command line:      Petrichor.exe commandName --logFile "[path]\logFile.txt"
Petrichor Script:  log-file : "[path]/logFile.txt"
```
Petrichor will generate the file `[path]\logFile.txt`.


---
# Running Petrichor via terminal

Call the executable ( `.exe` file ) via a terminal (command prompt) to run it.

If you run Petrichor with no arguments, it will prompt you to use the `--help` option to see available commands.

**Example:**

```powershell
[install path]> Petrichor.exe --help
```


---
# Running Petrichor via Batch file

If preferred, you can create a Batch file ( `.bat` file ) to run Petrichor for you.

1. Make a new text file, name it whatever you want, and change its extension to `.bat`.
	-  You can also open it in a text editor such as Notepad and use `save as → Batch file` to do the same thing.
2. Open the file in a text editor program, such as Notepad.
3. Type `start [install path]/Petrichor.exe]`, followed by command usage as shown above.
	- **NOTE:** [Relative paths](#relative-file-paths) are relative to the batch script by default. If they are used in Petrichor commands, they must be relative to Petrichor.exe instead, unless you [add Petrichor to your PATH environment variable](#adding-petrichor-to-windows-path-environment-variable).
4. Save the batch file.

Once you've done these steps, you can run the `.bat` file by double clicking it.

Assuming the `.bat` file was made correctly, it will run Petrichor with all the arguments you set.

**Example (Command in command line arguments):**

FOLDER CONTENTS:
```
- parent\
	- Petrichor\
		- Petrichor.exe
	- example batch file.bat
	- inputFile.txt
```
IN FILE `example batch file.bat`:
```batch
start Petrichor\Petrichor.exe commandName --inputFile [path]\inputFile.txt
```

**Example (Command in input file):**

FOLDER CONTENTS:
```
- parent\
	- Petrichor\
		- Petrichor.exe
	- example batch file.bat
	- inputFile.txt
```
IN FILE `example batch file.bat`:
```batch
start Petrichor\Petrichor.exe [path]\inputFile.txt
```

You can make a batch script wait for Petrichor to finish running and launch the output script automatically.

1. Create a batch script using the steps above.
2. After the `start` keyword, add `/wait`. This will cause the batch script to wait until Petrichor is closed before continuing.
3. Add a new line to the batch script, and enter `start [path/script.ahk]`.
4. Save the batch file.

Once you've done these steps, you can run the `.bat` file by double clicking it. Assuming the `.bat` file was made correctly, it will run Petrichor with all the arguments you set, wait until it closes, then launch the output script.

**NOTE:** If Petrichor fails to generate a new script, any existing version of the output script will be launched instead.

**Example (Command in command line arguments):**

FOLDER CONTENTS:
```
- parent\
	- Petrichor\
		- Petrichor.exe
	- example batch file.bat
	- inputFile.txt
```
IN FILE `example batch file.bat`:
```batch
start /wait Petrichor\Petrichor.exe commandName --inputFile ..\inputFile.txt
start outputFile.ahk
```

**Example (Command in input file):**

FOLDER CONTENTS:
```
- parent\
	- Petrichor\
		- Petrichor.exe
	- example batch file.bat
	- inputFile.txt
	- outputFile.ahk (will be generated after running)
```
IN FILE `example batch file.bat`:
```batch
start /wait Petrichor\Petrichor.exe ..\inputFile.txt
start outputFile.ahk
```


---
## Relative file paths

If you dont like having to get the full path for files, you can use relative paths instead.

`./` gets the folder the .exe file is in, and `../` gets the parent folder of that folder.

**Example:**

FOLDER CONTENTS:
```
- folder/
	- subfolder/
		- you are here.txt
		- example a.txt
	- example b.txt
```
PATH:
```
"./example a.txt" -> "folder\subfolder\example a.txt"
"../example b.txt" -> "folder\example b.txt"
```

---

## A note about slashes in file paths

On Windows, backslashes ( `\` ) and forward slashes ( `/` ) both work the same way. Use whichever you prefer to.

**NOTE:** They are not equivalent to each other in Petrichor Script.

---
## Adding Petrichor to Windows PATH environment variable

In order to run Petrichor without needing to navigate to its install folder or put its full install path into the terminal, you can add its install path to the Windows PATH environment variable.

1. Navigate to your Petrichor install folder.
2. Copy the folder path.
3. Open the Start Menu.
4. Search for "environment variables".
5. Open `Edit the system environment variables`.
6. Click the `Environment Variables` button.
7. Select the `Path` variable (Within the `User` variables to set locally, or within the `System` variables to set globally).
8. Click the `Edit` button.
9. Click the `New` button.
10. Paste the folder path you copied in step 2.
11. Click the `OK` button.

Once you do the above steps, you will be able to run Petrichor in a terminal directly without specifying its install path.

**Example:**
```txt
Without Petrichor in PATH variable:  C:\path\to\install\folder\Petrichor.exe commandName
With Petrichor in PATH variable:     Petrichor.exe commandName
```
