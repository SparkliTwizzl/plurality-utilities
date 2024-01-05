<div align="center"><image width="200" src="./logo.png"></div>
<h1 align="center">Petrichor</h1>
<h3 align="center">Version 0.9</h3>

---

[Source code and releases](https://github.com/SparkliTwizzl/plurality-utilities)

---

# Terms of use

By using this project or its source code, for any purpose and in any shape or form, you grant your implicit agreement to all the following statements:

- You attest that transgender and/or nonbinary people are people, rights for trans/NBi people are human rights which benefit all people, and trans/NBi people are fully capable of making decisions about their own lives.
- You attest that otherwise queer and/or LGBTQIA+ people are people, rights for queer/LGBTQIA+ people are human rights which benefit all people, and queer/LGBTQIA+ people are fully capable of making decisions about their own lives.
- You attest that autistic people are people, rights for autistic people are human rights which benefit all people, and autistic people are fully capable of making decisions about their own lives.
- You attest that otherwise neurodiverse people are people, rights for neurodiverse people are human rights which benefit all people, and neurodiverse people are fully capable of making decisions about their own lives.
- You condemn the unlawful invasion and/or occupancy of sovereign states, including but not limited to that by Russia to Ukraine.
- You condemn all forms of genocide against any peoples for any reason, including but not limited to that of the peoples of Palestine and western Sudan.

---

# 1 - What is it?

This tool is a command-line app with miscellaneous utilities. Currently wi're the only maintainer, so currently it only has stuff that wi feel the need to add to it.

---

# 2 - What does it do?

Currently the only utility it supports is generating AutoHotkey scripts to replace short sequences of text (tags) with longer strings with names, pronouns, and similar information. These scripts can be written by hand, but learning how to use AutoHotkey is required, and they're hard to make changes to if you ever want to, for example, change your name or your pronouns.

---

# 3 - AutoHotkey? What's that?

The scripts generated by the tool do nothing on their own. They are intended to be run with [AutoHotkey](https://www.autohotkey.com), a Windows scripting tool intended for automation, and without it, they're just a glorified text file.

---

# 4 - How do i use it?

In order to get a useful result from the tool, there are 3 main steps:

1. Write an input file.
2. Run the tool using the above.
3. Run the resulting script with AutoHotkey.

---

## 4.1 - Input file syntax

Input files are made up of data regions, which are made up of tokens. Some are required and some are optional.

---

### 4.1.1 - Data tokens and regions

All data in Petrichor input files is in the form of data tokens, which can be grouped into data regions.

#### 4.1.1.1 - Data tokens

The most basic element in input files is a data token, or simply a token.

Every token consists of a name and a value, separated by a `:`. Whitespace between and around these parts is ignored.

Token names are always in `lower-kebab-case`.

**Example (both tokens are identical to Petrichor):**

```ptcr
token-name:Token value.
 token-name : Token value. 
```

#### 4.1.1.2 - Data regions

Related tokens can be grouped into a data region, or simply a region. These consist of a token indicating the start of the region, then the region body, surrounded by brackets `{` / `}`.

**Example:**

```ptcr
region-name:
{
    token-1-in-region-body: Value.
    token-2-in-region-body: Value.
}
```

Regions can be contained within another region.

**Example:**

```ptcr
parent-region:
{
    token-a: Value.

    child-region:
    {
        token-b: Value.
    }
}
```

---

### 4.1.2 - Blank lines and comments

The token `#:` starts a comment which continues to the end of the line. The comment token is the only token which can be on the same line as other data.

Blank lines are ignored.

**Example:**

```ptcr
#: this is a comment. this line will be ignored. the following line is blank, and will also be ignored.

region:
{
    token: value #: this is an inline comment. everything after "#:" will be ignored.
}
```


---

### 4.1.3 - Metadata region (REQUIRED)

This region contains required information for Petrichor to run, and it must be the first region in the file.

#### 4.1.3.1 - Minimum version token (REQUIRED)

This token is required. It specifies the minimum Petrichor version required in order to parse the file.

**Example:**

```ptcr
metadata:
{
    minimum-version: major.minor.patch.preview
}
```

Major and minor version must be specified. If patch or patch and preview versions are blank, they are assumed to be any version.

**Example:**

```ptcr
minimum-version: 1.2.3.pre-4 #: Major version 1, minor version 2, patch version 3, preview version pre-4
minimum-version: 1.2.3 #: Major version 1, minor version 2, patch version 3, any preview version
minimum-version: 1.2 #: Major version 1, minor version 2, any patch or preview version
```

---

### 4.1.4 - Module options region (OPTIONAL)

This region is optional. It allows you to set up optional properties in the shortcut script if desired.

#### 4.1.4.1 - Custom icon tokens (OPTIONAL)

If desired, you can specify filepaths to custom icons for the shortcut script to use.

Available tokens are:

- `default-icon` : Min 0, max 1. This token allows setting a custom default icon for the script. Set value to the path to the icon file you want to use.
- `suspend-icon` : Min 0, max 1. This token allows setting a custom icon for the script to use when suspended. Set value to the path to the icon file you want to use.

**IMPORTANT NOTE:** If you move an icon file and do not update its path in your input file and regenerate the script, the icon will not be found by AutoHotkey and will not be applied.

**Example:**

```ptcr
module-options:
{
    default-icon: {path to default icon file}.ico
    suspend-icon: {path to suspend icon file}.ico
}
```

For simplicity, if an icon file will be in the same folder as the shortcut script, you can use a [relative path](#4.2.2---relative-file-paths).

**Example:**

```ptcr
module-options:
{
    default-icon: ./{default icon file name}.ico
    suspend-icon: ./{suspend icon file name}.ico
}
```

#### 4.1.4.2 - Reload / suspend shortcut tokens (OPTIONAL)

If desired, you can include keyboard shortcuts to reload and/or suspend the script.

To include a shortcut to reload the script, add a token to the module options region with the name `reload-shortcut` and set its value to a valid AutoHotkey v2.0 shortcut string; If you do not know how to write one, consult AutoHotkey documentation.

To include a shortcut to suspend the script, do the same with a token named `suspend-shortcut`.

**Example:**

```ptcr
module-options:
{
    reload-shortcut: #r #: Windows key + R
    suspend-shortcut: #s #: Windows key + S
}
```

---

### 4.1.5 - Entries region (REQUIRED)

This region defines the entries to be converted into macros. It must come before the `templates` region.

Each entry is an `entry` region which defines a set of values to create shortcut macros from. There is no limit to how many entries the `entries` region can contain.

#### 4.1.5.1 - Entry regions (OPTIONAL)

Entry regions are made up several token types. There are different restrictions and requirements for each type.

- `decoration` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
- `color` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
- `id` : Min 1, max 1. This token defines a value to associated with all `name` tokens that are present.
- `name` : Min 1. This token defines a name/tag pair.
    - This token type's value is structured as `[name] @[tag]`, where the `[name]` portion can be any non-blank string that does not contain an `@` character, and the `[tag]` portion can be any string that does not contain whitespace.
- `last-name` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
    - This token type's value structure is identical to that of `name` tags.
- `pronoun` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.

**NOTE:** All token values *should* be unique, even though Petrichor wont take issue with it. If a value is repeated, the AutoHotkey script generated from the input data will misbehave in unpredictable ways.

**Example:**

```ptcr
entry: #: all optional tokens present
{
    id: 1234
    name: Sam @sm
    name: Sammy @smy
    last-name: Smith @s
    pronoun: they/them
    color: #89abcd
    decoration: -- a person
}

entry: #: only required tokens present
{
    id: 4321
    name: ALEX @AX
}
```

---

### 4.1.6 - Templates region (REQUIRED)

This region defines the structure of generated macros. It must come after the entries region.

Templates define the structure of AutoHotkey macros to create from entries. There is no limit to how many templates can be used.

#### 4.1.6.1 - Template tokens (OPTIONAL)

Templates are defined by tokens with the name `template` and a valid AutoHotkey hotstring. Consult AutoHotkey documentation if you do not know how to write one. A basic overview is provided here.

All templates must start with a `find` text string, then `::`, then a `replace` text string.

**NOTE:** You cannot use `::` in a `find` string due to the way AutoHotkey hotstrings work.

These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by inserting a backtick `` ` `` at the start or end of the `find` and/or `replace` strings.

Use [marker strings](#4.1.6.1.1---template-marker-strings) to define how templates should be applied to entries.

If this is not followed, the generated script wont work correctly, even though Petrichor will run without errors.

**Example:**

```ptcr
templates:
{
    template: [find string] :: ` [replace string] `
}

#: MACROS GENERATED FROM INPUT:

::[find string]::` [replace string] `
```

##### 4.1.6.1.1 - Template marker strings

Certain symbols will be replaced by fields from entries in the input file by default. This is how templates are able to be used to generate macros.

If no marker strings are present, a template will be inserted into the output file with no changes for every `name` token present in the input file. This technically will not break the script, but it is not recommended.

Available marker strings are:

- `[color]`
- `[decoration]`
- `[id]`
- `[name]`
- `[last-name]`
- `[last-tag]`
- `[pronoun]`
- `[tag]`

**NOTE:** Only these supported marker strings can be used. Unknown marker strings will be rejected.

**NOTE:** By default, you cannot use the `[` or `]` symbols in a template string. Use [escape characters](#41612---escape-characters) to circumvent this.

**Example:**

```ptcr
entries:
{
    entry:
    {
        id: 1234
        name: Sam @sm
        name: Sammy @smy
        last-name: Smith @s
        pronoun: they/them
        color: #89abcd
        decoration: -- a person
    }
}

templates:
{
    template:  [tag][last-tag] :: [id] - [name] [last-name] ([pronoun]) | {[decoration]} | [color]
}

#: MACROS GENERATED FROM INPUT:

::sms::1234 - Sam Smith (they/them) | {-- a person} | #89abcd
::smys::1234 - Sammy Smith (they/them) | {-- a person} | #89abcd
```

You can use each marker string in a template as many times as you want

**Example:**

```ptcr
entries:
{
    entry:
    {
        id: 1234
        name: Sam @sm
        name: Sammy @smy
        last-name: Smith @s
        pronoun: they/them
        color: #89abcd
        decoration: -- a person
    }
}

templates:
{
    template: [tag][tag] :: [name] | {[name] [decoration]}
}

#: MACROS GENERATED FROM INPUT:

::smsm::Sam (they/them) | [Sam is a person]
::smysmy::Sammy (they/them) | [Sammy is a person]
```

##### 4.1.6.1.2 - Escape characters

Backslash `\` is treated as an "escape character" in templates. It is used to disable the normal function of special characters. An escape character can be applied to another escape character in order to make the scond one print literally.

**Example:**

```ptcr
entries:
{
    entry:
    {
        id: 1234
        name: Sam @sm
        name: Sammy @smy
        last-name: Smith @s
        pronoun: they/them
        color: #89abcd
        decoration: -- a person
    }
}

templates:
{
    template: [tag] :: [name] \\ \[[decoration]\]
}

#: MACROS GENERATED FROM INPUT:

::sm::Sam \ [a person]
::smy::Sammy \ [a person]
```

---

## 4.2 - Running the tool

Call the executable (`.exe` file) via command line to run it

It may be easier to write a batch script (.bat file) to do this for you (see below for how to do this). If you call it with no arguments, it will show helptext explaining how to use it.

**Example:**

```
{installpath}\Petrichor\
>Petrichor.exe
```

### 4.2.1 - `generateAHKShortcutScript` command

To generate an AutoHotkey shortcut script, call Petrichor with the command argument `generateAHKShortcutScript`. This command has several options, some of which are required, which are explained below.

**Example:**

```
{installpath}\Petrichor\
>Petrichor.exe generateAHKShortcutScript
```

#### 4.2.1.1 - `--inputFile` option (REQUIRED)

Add the `--inputFile` option to the `generateAHKShortcutScript` command and pass the path to the input file after it. You must include the path and the file extension for the input file. [Relative file paths](#4.2.2---relative-file-paths) can be used.

**Example:**

```
{installpath}\Petrichor\
>Petrichor.exe generateAHKShortcutScript --inputFile "{path}\inputFile.txt"
```

#### 4.2.1.2 - `--outputFile` option (OPTIONAL)

Add the `--outout` option to the `generateAHKShortcutScript` command and pass the output file after it.

The output file can be either a full filepath or just a filename. In the case of the latter, the output file will be created in a default location.

A file extension is not required, and if one is included it will be replaced with `.ahk` automatically. [Relative file paths](#4.2.2---relative-file-paths) can be used.

**Example (full filepath, specify output location):**

```
{installpath}\Petrichor\
>Petrichor.exe generateAHKShortcutScript --inputFile {input file} --outputFile "C:\path\to\output\file\outputFile"
```

RESULT:

```
C:\path\to\output\file\outputFile.ahk ← will be generated by Petrichor
```

**Example (filename only, default output location):**

```
{installpath}\Petrichor\
>Petrichor.exe generateAHKShortcutScript --inputFile {input file} --outputFile "outputFile"
```

RESULT:

```
{installpath}\Petrichor\_output\outputFile.ahk ← will be generated by Petrichor
```

#### 4.2.1.3 - `--logMode` option (OPTIONAL)

This option is used to control where logs are sent.

Values for this option are:

- `none` - disable logging (default)
- `consoleOnly` - send logs only to console output
- `fileOnly` - send logs onto to log file
- `all` - send logs to all output locations

#### 4.2.1.4 - `--logFile` option (OPTIONAL)

This option is used to customize the file name and/or location to generate a log file at.

NOTE: log file will only be created if logging to file is enabled (see 4.2.2.3).

**Example (filename only, default directory):**

```
Petrichor.exe generateAHKShortcutScript --inputFile {input file} --logMode all --logFile logFile.txt
```

RESULT:

```
{path}/Petrichor/_log/logFile.txt ← will be generated by Petrichor
```

**Example (directory path only, default filename):**

```
Petrichor.exe generateAHKShortcutScript --inputFile {input file} --logMode all --logFile {log path}/
```

RESULT:

```
{log path}/{yyyy}-{MM}-{dd}_{HH}-{mm}-{ss}.log ← will be generated by Petrichor
```

**Example: (full filepath, no defaults):**

```
Petrichor.exe generateAHKShortcutScript --inputFile {input file} --logMode all --logFile {log path}/logFile.txt
```

RESULT:

```
{log path}/logFile.txt ← will be generated by Petrichor
```

### 4.2.2 - Relative file paths

If you dont like having to get the full path for files, you can use relative paths instead.

`./` gets the folder the .exe file is in, and `../` gets the parent folder of that folder.

**Example:**

FOLDER CONTENTS:

```
- parent/
    - Petrichor/
        - outputFile.ahk (will be generated after running)
        - Petrichor.exe
    - inputFile.txt
```

IN COMMAND PROMPT:

```
{installpath}\Petrichor\
>Petrichor.exe generateAHKShortcutScript --inputFile ../inputFile.txt --outputFile ./outputFile
```

### 4.2.3 - A note about slashes in file paths on Windows

On Windows, backslashes `\` and forward slashes `/` both work the same way. Use whichever you prefer to. **They are not equivalent to each other in input files, however.**

### 4.2.4 - Running Petrichor via batch script (`.bat` file)

If you're going to run the tool with the same arguments every time, it's much simpler to write a simple .bat file to run the tool for you.

1. Make a new text file, name it whatever you want, and change its extension to `.bat`.
    -  You can also open it in a text editor such as Notepad and use `save as → Batch file` to do the same thing.
2. Open the file in a text editor program, such as Notepad.
3. Type `start {installpath}/Petrichor.exe]`, followed by command usage as shown above.
    - **NOTE:** [Relative paths](#4.2.2---relative-file-paths) are relative to the batch script by default. If relative paths are used in Petrichor commands, they must be relative to Petrichor.exe instead.
4. Save the batch file.

Once you've done these steps, you can run the `.bat` file by double clicking it. Assuming the .bat file was made correctly, it will run Petrichor with all the arguments you set.

**Example:**

FOLDER CONTENTS:

```
- parent\
    - Petrichor\
        - Petrichor.exe
    - example batch file.bat
    - inputFile.txt
    - outputFile.ahk (will be generated after running)
```

IN FILE "example batch file.bat":

```batch
start Petrichor\Petrichor.exe generateAHKShortcutScript --inputFile ..\inputFile.txt --outputFile ..\outputFile
```

Optionally, you can make a batch script wait for Petrichor to finish running and launch the output script automatically.

1. Create a batch script using the steps above.
2. After the `start` keyword, add `/wait`. This will cause the batch script to wait until Petrichor is closed before continuing.
3. Add a new line to the batch script, and enter `start [path/script.ahk]`.
4. Save the batch file.

Once you've done these steps, you can run the `.bat` file by double clicking it. Assuming the .bat file was made correctly, it will run Petrichor with all the arguments you set, wait until it closes, then launch the output script.

**NOTE:** If Petrichor fails to generate a new script, any existing version of the output script will be launched instead.

**Example:**

FOLDER CONTENTS:

```
- parent\
    - Petrichor\
        - Petrichor.exe
    - example batch file.bat
    - inputFile.txt
    - outputFile.ahk (will be generated after running)

```

IN FILE "example batch file.bat":

```batch
start /wait Petrichor\Petrichor.exe generateAHKShortcutScript --inputFile ..\inputFile.txt --outputFile ..\outputFile
start outputFile.ahk
```

---

# 5 - Using the script generated by Petrichor

## 5.1 - Install AutoHotkey

Before you can do anything with your script, you need to install AutoHotkey. Download and install AutoHotkey v2 [here](https://www.autohotkey.com) and install it, then continue.

## 5.2 - Running the script

Either double-click the .ahk file or right click on it and click "run script" in the dropdown menu.

## 5.3 - Methods to run the script automatically

If you get sick of it, there are a few options.

### 5.3.1 - Windows Startup shortcut

This is the simplest method. It's not totally reliable, but it works most of the time. Occasionally a script will launch successfully, but not show up in the taskbar tray. If that bothers you, just relaunch the script manually.

Here's how to do it:

1. Right-click the script.
2. Click `Create shortcut` in the dropdown menu.
3. Press `Win+R` to open the Windows Run dialog.
4. Type `shell:startup` into the dialog, then click OK.
5. The Startup folder will open. Copy the shortcut you created in step 2 into it.

### 5.3.2 - Task Scheduler

Wi've found this method to be less reliable than the Windows Startup method, but it does work more often than not. It's also kind of a pain to set up. Wi recommend using the Windows Startup method over this one, unless that method doesnt work for you.

You can follow the directions [here](https://windowsloop.com/run-autohotkey-script-at-windows-startup/) to set it up.

### 5.3.3 - Registry (NOT RECOMMENDED)

**DO NOT DO THIS UNLESS YOU KNOW WHAT YOU'RE DOING. Editing the registry can brick your computer if you're not careful.**

Wi strongly recommend using one of the other methods above, unless all of them dont work for you.

Also, wi havent personally tested this method, so wi dont know how reliable it is, but it probably should work about the same as the other two?

1. Open the Registry Editor. There are two days to do this:
	- Press `Win+R` to open the Run dialog, type in `regedit`, then click OK.
	- Open the Start menu and search for either `regedit` or `Registry Editor`.
2. Navigate to `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`.
3. Add a new String key. Name it however you prefer.
4. Edit the value of the new string key and put in `"@:\path\to\autohotkey\version\file.exe" "@:\path\to\script\file.ahk"`, using the filepaths of your AutoHotkey installation and your script file.

---

# 6 - I think i found a bug / I have an idea for the project

Report bugs and make suggestions here: [GitHub issues board](https://github.com/SparkliTwizzl/plurality-utilities/issues)

If there's a dead link in this documentation, please report it so it can be fixed.

In order for developers to find bugs easier, please describe what you were doing in as much detail as you're able to (even better, write steps to reproduce the issue), say what you expected to happen, say what actually happened, and if you can, include the log file.
