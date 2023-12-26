<div align="center"><image width="200" src="./logo.png"></div>
<h1 align="center">Petrichor</h1>
<h3 align="center">Version 0.8</h3>

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

## 1 - What is it?

This tool is a command-line app with miscellaneous utilities. Currently wi're the only maintainer, so currently it only has stuff that wi feel the need to add to it.

---

## 2 - What does it do?

Currently the only utility it supports is generating AutoHotkey scripts to replace short sequences of text (tags) with longer strings with names, pronouns, and similar information. These scripts can be written by hand, but learning how to use AutoHotkey is required, and they're hard to make changes to if you ever want to, for example, change your name or your pronouns.

---

## 3 - AutoHotkey? What's that?

The scripts generated by the tool do nothing on their own. They are intended to be run with [AutoHotkey](https://www.autohotkey.com), a Windows scripting tool intended for automation, and without it, they're just a glorified text file.

---

## 4 - How do i use it?

In order to get a useful result from the tool, there are 3 main steps:

1. Write an input file.
2. Run the tool using the above.
3. Run the resulting script with AutoHotkey.

---

### 4.1 - Input files

Input files are made up of 3 regions: Metadata, entries, and templates.

Blank lines in input files are ignored, so use as many as you want.

---

#### 4.1.1 - Metadata

This region is optional. It allows you to set custom icons on the shortcut script if desired.

##### 4.1.1.1 - Custom icons

If desired, you can specify filepaths to custom icons for the shortcut script to use.

To set the default icon, add a token to the metadata region called `default-icon` and set its value to the path to the icon file you want to use.

If you want a different icon to be used when the script is suspended, add a token to the metadata region called `suspend-icon` and set its value to the path to the icon file you want to use.



**IMPORTANT NOTE:** If you move an icon file and do not update its path in your input file and regenerate the script, the icon will not be found by AutoHotkey and will not be applied.



Example:

```
metadata:
{
    default-icon: {path to default icon file}.ico
    suspend-icon: {path to suspend icon file}.ico
}
```



For simplicity, if an icon file will be in the same folder as the shortcut script, you can use a relative path (see 4.2.3 below for more on how relative paths work).



Example:

```
metadata:
{
    default-icon: ./{default icon file name}.ico
    suspend-icon: ./{suspend icon file name}.ico
}
```

##### 4.1.1.2 - Keyboard shortcut to reload script

If desired, you can include a keyboard shortcut to reload the script.

To include a reload shortcut, add a token to the metadata region called `reload-shortcut` and set its value to a valid AutoHotkey v2.0 shortcut string; If you do not know how to write one, consult AutoHotkey documentation.

Example:

```
metadata:
{
    reload-shortcut: #r ; Windows key + R
}
```

##### 4.1.1.3 - Keyboard shortcut to suspend script

If desired, you can include a keyboard shortcut to toggle suspending the script. Suspending the script will prevent macros from working until it is resumed.

To include a suspend shortcut, add a token to the metadata region called `suspend-shortcut` and set its value to a valid AutoHotkey v2.0 shortcut string; If you do not know how to write one, consult AutoHotkey documentation.

Example:

```
metadata:
{
    suspend-shortcut: #s ; Windows key + S
}
```

---

#### 4.1.2 - Entries are blocks of data which are made up of fields.

Each entry represents a person and must contain at least one identity (a name paired with a tag).

To write an entry, start with an open curly brace `{` on one line and a close curly brace `}` on another, with nothing else on those lines.



Example:

```
{
}
```

##### 4.1.2.1 - Between the braces, write the fields for the entry on separate lines.

Whitespace at the start of lines for fields is ignored, so feel free to indent or not as you prefer to.

Fields are identified by marker symbols:

- `#name#`
  - Name fields are a sub-field of identity fields (see below).
  - Name fields must be surrounded on both sides by hash symbols `#`.
  - Note that this means that name fields cannot contain hash symbols.
- `@tag`
  - Tag fields are a sub-field of identity fields (see below).
  - Tag fields cannot contain spaces.
- `$pronoun`
  - Pronoun fields are optional.
  - Entries cannot have more than one pronoun field.
- `&decoration`
  - Decoration fields are optional.
  - Entries cannot have more than one decoration field.

Identity fields are a special case, as they consist of pairs of name and tag fields:

- `% #name# @tag`
  - Every entry must contain at least one identity field.
  - There is no upper limit to how many identity fields an entry can have.



Example:

```
{
  % #Sam# @sm
  $they/them
  &-- a person
}
```

##### 4.1.2.2 - There's no limit on how many entries an input file can have, and entries and fields dont have to be unique.

If you want to, for example, have the same set of names paired with a different pronoun and/or decoration, you can include multiple entries that are the same aside from small changes (see below).

**IMPORTANT NOTE:** All tag fields *should* be unique in order for the generated script to work correctly, even though Petrichor wont take issue with it. If a tag field is repeated, only the first one in the script will work.



Example:

```
entries:
{
  {
    % #Sam# @sm
    % #Sammy# @smy
  }
  {
    % #Sam# @sm-t
    % #Sammy# @smy-t
    $they/them
    &-- a person
  }
  {
    % #Sam# @sm-h
    % #Sammy# @smy-h
    $he/him
    &-- a person
  }
  {
    % #Sam# @sm-s
    % #Sammy# @smy-s
    $she/her
    &-- a person
  }
}
```

---

#### 4.1.3 - Templates are how the tool converts entries into AutoHotkey macros.

In order for Petrichor to know what format(s) you want the macros in your script to have, you need to provide templates for them.

##### 4.1.3.1 - Templates must use the same basic structure in order for the generated script to work.

All templates have to start with 2 colons `::`, a string of text including an at sign `@` representing the tag, then 2 more colons `::`.

The tag string can be anything you want, as long as it contains at least one at sign `@` and no spaces. Additional text is optional.

If this is not followed, the generated script wont work, even though Petrichor will run without errors.



Example:

```
templates:
{
  ::@:: ← the rest of the template must come after this
}
```

##### 4.1.3.2 - Templates must contain marker symbols for the tool to replace in order for them to do anything.

Certain symbols will be replaced by fields from entries in the input file by default. This is how templates are able to be used to generate macros.

Below is a list of the marker symbols and the fields they will be replaced by when the tool runs:

- `# → name`
- `@ → tag`
- `$ → pronoun`
- `& → decoration`



Example:

```
entries:
{
  {
    % #Sam# @sm
    % #Sammy# @smy
    $they/them
    &-- a person
  }
}

templates:
{
  ::@::# ($) | [&]
}
```

This produces this output file:

```
::sm::Sam (they/them) | [-- a person]
::smy::Sammy (they/them) | [-- a person]
```

##### 4.1.3.3 - You can use each marker symbol in a template as many times as you want.



Example:

```
entries:
{
  {
    % #Sam# @sm
    % #Sammy# @smy
    $they/them
    & is a person
  }
}

templates:
{
  ::@@::# ($) | [#&]
}
```

This produces this output file:

```
::smsm::Sam (they/them) | [Sam is a person]
::smysmy::Sammy (they/them) | [Sammy is a person]
```

##### 4.1.3.4 - You can use a backslash `\`, aka an "escape character", to use marker symbols without them being replaced.

Note that you can apply an escape character to a backslash in order to make it print literally.



Example:

```
entries:
{
  {
    % #Sam# @sm
    % #Sammy# @smy
    $they/them
    &a person
  }
}

templates:
{
  ::\@@::# ($) \\ [\#&]
}
```

This produces this output file:

```
::@sm::Sam (they/them) \ [#a person]
::@smy::Sammy (they/them) \ [#a person]
```

##### 4.1.3.5 - The templates region can have as many templates as you want.

Although templates dont have to be unique, repeating a template will generate duplicate macros, which could break the generated script.



Example:

```
entries:
{
  {
    % #Sam# @sm
    % #Sammy# @smy
    $they/them
    &-- a person
  }
  {
    % #Alex# @ax
    $it/its
    &[a person too]
  }
  {
    % #Raven# @rvn
    $thon/thons>they/them
  }
  {
    % #Beck# @bk
  }
}

templates:
{
  ::\@@::#
  ::\@@-::# ($)
  ::<\@@>::# ($) | [&]
}
```

This produces this output file:

```
::@sm::Sam
::@sm-::Sam (they/them)
::<@sm>::Sam (they/them) | [-- a person]
::@smy::Sammy
::@smy-::Sammy (they/them)
::@smy::Sammy (they/them) | [-- a person]
::@ax::Alex
::@ax-::Alex (it/its)
::@ax::Alex (it/its) | [[a person too]]
::@rvn::Raven
::@rvn-::Raven (thon/thons>they/them)
::@rvn::Raven (thon/thons>they/them) || []
::@bk::Beck
::@bk-::Beck ()
::@bk::Beck () || []
```

---

### 4.2 - Running the tool

#### 4.2.1 - Call the executable (.exe file) via command line to run it.

It's easier to write a batch script (.bat file) to do this for you (see below for how to do this). If you call it with no arguments, it will show helptext explaining how to use it.



Example:

```
C:\path\to\tool\folder\Petrichor\
>Petrichor.exe
```

#### 4.2.2 - In order to generate an AutoHotkey script with the tool, you need an input file and a templates file (see above for how to write them).

Pass the path to the input file as the first argument (arg0) and the path to where you want the output file to be generated as the second argument (arg1).

You must include the file extension for the input file, but the extension on the output file will be ignored and is irrelevant.

**IMPORTANT NOTE:** If you pass the path to an existing output file, it will be overwritten.



Example:

```
C:\path\to\tool\folder\Petrichor\
>Petrichor.exe C:\path\to\input\file\input.txt C:\path\to\output\file\output
```

#### 4.2.3 - If you dont like having to get the full path for files, you can use relative paths instead.

`./` gets the folder the .exe file is in, and `../` gets the parent folder of that folder.



Example:

```
folder contents:

- parent/
  - Petrichor/
    - output.ahk (will be generated after running)
    - Petrichor.exe
  - input.txt


in command prompt:

C:\path\to\parent\Petrichor\
>Petrichor.exe ../input.txt ./output
```

#### 4.2.4 - A note about slashes in paths:

On Windows, backslashes `\` and forward slashes `/` both work the same way. Use whichever you prefer to. They are not equivalent to each other in input files, however.

#### 4.2.5 - You can pass just a filename for the output file and it will be generated in a default location.

If you pass a filename with no path, the file will be generated in a folder called `_output` inside the tool's folder.



Example:

```
folder contents:

- parent/
  - Petrichor/
    - _output/
      - output.ahk (will be generated after running)
    - Petrichor.exe
  - input.txt


in command prompt:

C:\path\to\parent\Petrichor\
>Petrichor.exe ../input.txt output
```

#### 4.2.6 - Optionally, you can pass a third argument to enable logging.

Pass `-l` as the third argument (arg2) to enable logging in basic mode (writes log info to log file only), or `-v` to enable logging in verbose mode (writes log info to log file and to the console window).

Log files are found in a folder called `_log` inside the tool's folder. Log file names are automatically generated using the date and time when the tool is run.



Example:

```
folder contents:

- parent\
  - Petrichor\
    - _log
      - yyyy-MM-dd_HH-mm-ss.log (will be generated after running)
    - Petrichor.exe
  - input.txt
  - output.txt


in command prompt:

C:\path\to\parent\folder\Petrichor\
>Petrichor.exe ../input.txt ../output -l
```

#### 4.2.7 - You can make a batch script (.bat file) to run the tool for you.

If you're going to run the tool with the same arguments every time, it's much simpler to write a simple .bat file to run the tool for you.

##### 4.2.7.1 - Make a new text file, name it whatever you want, and change its extension to .bat.

You can also open it in a text editor such as Notepad and use `save as → Batch file` to do the same thing.

##### 4.2.7.2 - Open the file in a text editor program, such as Notepad.

##### 4.2.7.3 - In the file, put in the command usage as shown above, then save it.



Example:

```
folder contents:

- parent\
  - Petrichor\
    - _log\
      - (log files will be generated here after running the tool if logging is enabled)
    - Petrichor.exe
  - example batch file.bat
  - input.txt
  - output.ahk (will be generated after running the tool)


in file "example batch file.bat":

Petrichor/Petrichor.exe ./input.txt ./output -v
```

##### 4.2.7.4 - Once you've done all that, run the .bat file by double clicking it.

Assuming the .bat file was made correctly, it will run Petrichor with all the arguments you set.

---

### 4.3 - Using the script generated by Petrichor

#### 4.3.1 - Before you can do anything with your script, you need to install AutoHotkey.

Download it [here](https://www.autohotkey.com) and install it, then continue.

#### 4.3.2 - Now that AutoHotkey is installed, you need to run the script.

Either double-click the .ahk file or right click on it and click "run script" in the dropdown menu.

#### 4.3.3 - Having to launch the script every time you boot your computer can get annoying.

If you get sick of it, you can follow the directions [here](https://windowsloop.com/run-autohotkey-script-at-windows-startup/) to make it run automatically. It might still sometimes fail to launch, but it works the majority of the time.

---

## 5 - I think i found a bug? / I have an idea for the project.

Report bugs and make suggestions here: [GitHub issues board](https://github.com/SparkliTwizzl/plurality-utilities/issues)

If there's a dead link in this documentation, please report it so it can be fixed.

In order for developers to find bugs easier, please describe what you were doing in as much detail as you're able to (even better, write steps to reproduce the issue), say what you expected to happen, say what actually happened, and if you can, include the log file.
