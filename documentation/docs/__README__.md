<div align="center"><image width="200" src="./logo.png"></div>
<h1 align="center">Petrichor</h1>
<h4 align="center">/ˈpɛtrɪˌkɔːr/ (noun) The scent of damp earth, particularly after rain.</h4>

<div align="center">Part of the <a href="https://github.com/SparkliTwizzl/trioxichor">Trioxichor project</a>.</div>

---
# About

Petrichor is a command-line app with modules for various utilities.

[Source code and releases](https://github.com/SparkliTwizzl/petrichor)

Petrichor is licensed under the [Anti-Exploitation License Noncommercial Attribution v1.1 (AEL-NC-AT 1.1)](https://github.com/SparkliTwizzl/anti-exploitation-license). By using this project or its source material, you agree to abide by the terms of this license.

---
# I think i found a bug / I have an idea for the project

Report bugs and make suggestions here: [GitHub issues board](https://github.com/SparkliTwizzl/plurality-utilities/issues)

If there's a dead link in this documentation, please report it so it can be fixed.

In order for developers to find bugs easier, please describe what you were doing in as much detail as you're able to (even better, write steps to reproduce the issue), say what you expected to happen, say what actually happened, and if you can, include the log file.

---

# 5 - Module-specific information

## 5.1 - Text Shortcut Script Generation module

### 5.1.1 - Text Shortcut Script Generation module tokens

This section details all the [data tokens](#411---data-tokens-and-regions) that are [module-specific](#423---module-specific-tokens) for the Text Shortcut Script Generation module.

This information likely won't make sense unless you [know how to use it](#4---how-do-i-use-it) first.

---

#### 5.1.1.1 - Module options region (OPTIONAL)

This variant of the [module options region](#422---module-options-region-optional) includes options supported by the [Text Shortcut Script Generation module](#51---text-shortcut-script-generation-module).

---

##### 5.1.1.1.1 - Custom icon tokens (OPTIONAL)

If desired, you can specify filepaths to custom icons for the shortcut script to use.

Available tokens are:

- `default-icon` : Min 0, max 1. This token allows setting a custom default icon for the script. Set value to the path to the icon file you want to use.
- `suspend-icon` : Min 0, max 1. This token allows setting a custom icon for the script to use when suspended. Set value to the path to the icon file you want to use.

**IMPORTANT NOTE:** If you move an icon file and do not update its path in your input file and regenerate the script, the icon will not be found by AutoHotkey and will not be applied.

**Example:**

```petrichor
module-options:
{
    default-icon: {path to default icon file}.ico
    suspend-icon: {path to suspend icon file}.ico
}
```

For simplicity, if an icon file will be in the same folder as the shortcut script, you can use a [relative path](#433---relative-file-paths).

**Example:**

```petrichor
module-options:
{
    default-icon: ./{default icon file name}.ico
    suspend-icon: ./{suspend icon file name}.ico
}
```

---

##### 5.1.1.1.2 - Reload / suspend shortcut tokens (OPTIONAL)

If desired, you can include keyboard shortcuts to reload and/or suspend the script.

To include a shortcut to reload the script, add a token to the module options region with the name `reload-shortcut` and set its value to a valid AutoHotkey v2.0 shortcut string; If you do not know how to write one, consult AutoHotkey documentation. To make this easier, some [find and replace strings](#511121---shortcut-find-and-replace-strings) are supported by Petrichor.

To include a shortcut to suspend the script, do the same with a token named `suspend-shortcut`.

**Example:**

```petrichor
module-options:
{
    reload-shortcut: #r // Windows key + R
    suspend-shortcut: #s // Windows key + S
}
```

---

###### 5.1.1.1.2.1 - Shortcut find-and-replace strings

The following strings are supported:

- `[windows]` / `[win]` → Windows key
- `[alt]` → either Alt key
- `[left-alt]` / `[lalt]` → left Alt key
- `[right-alt]` / `[ralt]` → right Alt key
- `[control]` / `[ctrl]` → either Control key
- `[left-control]` / `[lctrl]` → left Control key
- `[right-control]` / `[rctrl]` → right Control key
- `[shift]` → either Shift key
- `[left-shift]` / `[lshift]` → left Shift key
- `[right-shift]` / `[rshift]` → right Shift key
- `[and]` → `&`
- `[alt-graph]` / `[altgr]` → AltGr (AltGraph) key
- `[wildcard]` / `[wild]` → `*`
- `[passthrough]` / `[tilde]` → `~`
- `[send]` → `$`
- `[tab]` → Tab key
- `[caps-lock]` / `[caps]` → CapsLock key
- `[enter]` → Enter key
- `[backspace]` / `[bksp]` → Backspace key
- `[insert]` / `[ins]` → Insert key
- `[delete]` / `[del]` → Delete key
- `[home]` → Home key
- `[end]` → End key
- `[page-up]` / `[pgup]` → PageUp key
- `[page-down]` / `[pgdn]` → PageDown key
- `\[` → `[`
- `\]` → `]`

**Example:**

```petrichor
module-options:
{
    reload-shortcut: [win]r // Windows key + R
    suspend-shortcut: \[win\]s // [win]s
}
```

---

#### 5.1.1.2 - Shortcut list region (REQUIRED)

This region defines the text shortcuts to be generated.
Standard or templated shortcuts can be defined.
Templated shortcuts define the structure of shortcuts to create from entries.

---

##### 5.1.1.2.1 - Shortcut tokens (OPTIONAL)

This token defines a standard plaintext shortcut.
Standard shortcuts are defined by tokens with the name `shortcut`.
Shortcuts can use special AutoHotkey behavior if written correctly. Consult AutoHotkey documentation to learn how to do this.

All shortcuts must start with a "find" text string, then `::`, then a "replace" text string.
**NOTE:** You cannot use `::` in a "find" string due to the way AutoHotkey hotstrings work. Petrichor will allow it, but the shortcuts it generates will not work.
These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by inserting a backtick `` ` `` at the start or end of the "find" and/or "replace" strings.

**Example:**

```petrichor
shortcut-list:
{
    shortcut: [find string] :: ` [replace string] `
}

// SHORTCUTS GENERATED FROM INPUT:

// Standard shorcuts will only be written to the output file once.
::[find string]::` [replace string] ` // This is a standard shortcut. The [find string] and [replace string] will be inserted into the output file unaltered.
```

---

##### 5.1.1.2.2 - Shortcut template region (OPTIONAL)

This region defines a template to create shortcuts from.
Templated shortcuts are defined by tokens with the name `shortcut-template`.

All shortcuts must start with a "find" text string, then `::`, then a "replace" text string.
**NOTE:** You cannot use `::` in a "find" string due to the way AutoHotkey hotstrings work. Petrichor will allow it, but the shortcuts it generates will not work.
These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by inserting a backtick `` ` `` at the start or end of the "find" and/or "replace" strings.

Use [marker strings](#511221---template-marker-strings) to define how templates should be applied to entries.
Templates support [custom find and replace dictionaries](#511222---find-and-replace-tokens). 

If this is not followed, the generated script wont work correctly, even though Petrichor will run without errors.

**Example:**

```petrichor
shortcut-list:
{
    shortcut-template: [find string] :: ` [replace string] ` // No optional features are used, so no region body is needed.
    shortcut-template: [find string] :: [replace string] // Optional features are used, so a region body is needed.
    {
        // optional feature tokens go here.
    }
}

// SHORTCUTS GENERATED FROM INPUT:

// One copy of these shortcuts will be generated for every entry. Any template markers in the [find string] or [replace string] will be replaced with entry data.
::[find string]::` [replace string] ` // This is a standard template. No extra processing will be done to it.
::[find string]::[replace string] // Optional features will be applied to this template.
```

---

###### 5.1.1.2.2.1 - Template marker strings

Certain symbols will be replaced by fields from entries in the input file by default. This is how templates are able to be used to generate shortcuts.

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

**NOTE:** By default, you cannot use the `[` or `]` symbols in a template string. Use [escape characters](#413---escape-characters) to circumvent this.

**Example:**

```petrichor
entry-list:
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

shortcut-list:
{
    shortcut-template:  [tag][last-tag] :: [id] - [name] [last-name] ([pronoun]) | {[decoration]} | [color]
}

// shortcutS GENERATED FROM INPUT:

::sms::1234 - Sam Smith (they/them) | {-- a person} | #89abcd
::smys::1234 - Sammy Smith (they/them) | {-- a person} | #89abcd
```

You can use each marker string in a template as many times as you want

**Example:**

```petrichor
entry-list:
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

shortcut-list:
{
    shortcut-template: [tag][tag] :: [name] | {[name] [decoration]}
}

// shortcutS GENERATED FROM INPUT:

::smsm::Sam (they/them) | [Sam is a person]
::smysmy::Sammy (they/them) | [Sammy is a person]
```

---

###### 5.1.1.2.2.2 - Find and replace tokens (OPTIONAL)

These tokens are used to define custom find-and-replace pairs for a template.

The find-and-replace pairs are only applied to the template's "replace" string.
They are applied after a template is populated with entry data, and therefore are able to modify that data.

"Find" keys are defined by a token with the name `find`.
"Replace" values are defined by a token with the name `replace`.

The value of a `find` or `replace` token must be an inline region body, starting with a `{` and ending with a `}`.
Within the token body, put your find/replace items as a comma-separated list.
The lists cannot contain blank items, and they must have the same number of items as each other.

A `replace` token must be paired with a `find` token, and must come after it.
A `find` token can be present without a paired `replace` token, but this will cause all the "find" items to be removed rather than replaced with other text.

**Example:**

```petrichor
shortcut-list:
{
    shortcut-template: [find string] :: [replace string] custom find 1, custom find 2
    {
        find: { custom find 1, custom find 2 } // These are the "find" keys.
        replace: { replace 1, replace 2 } // These are the corresponding "replace" values.
    }
}

// shortcutS GENERATED FROM INPUT:

::[find string]::[replace string] replace 1, replace 2 // If the "find" keys are present in the [replace string] values for an entry, they will be replaced as well.
```

---

###### 5.1.1.2.2.2 - Text Case tokens (OPTIONAL)

These tokens are used to change the text case of a template's "replace" string.
Case conversion is applied after [custom find-and-replace pairs](#511222---find-and-replace-tokens).

Text case is defined by a token with the name `text-case`.
Allowed values are:
- unchanged (as-written; default)
- upper (UPPER CASE)
- lower (lower case)
- firstCaps (First Capitals Case)

**Example:**

```petrichor
shortcut-list:
{
    shortcut-template: [find string] :: [replace String]
    {
        text-case: unchanged
    }
    shortcut-template: [find string] :: [replace String]
    {
        text-case: upper
    }
    shortcut-template: [find string] :: [replace String]
    {
        text-case: lower
    }
    shortcut-template: [find string] :: [replace String]
    {
        text-case: firstCaps
    }
}

// shortcutS GENERATED FROM INPUT:

::[find string]::[replace String]
::[find string]::[REPLACE STRING]
::[find string]::[replace string]
::[find string]::[Replace String]
```

---

#### 5.1.1.3 - Entry list region (REQUIRED)

This region defines the entries to be converted into shortcuts.

Each entry is an `entry` region which defines a set of values to create shortcut shortcuts from. There is no limit to how many entries the `entry-list` region can contain.

---

##### 5.1.1.3.1 - Entry regions (OPTIONAL)

This region defines a set of data to apply shortcut templates to. At least one of this region must be present.

Entry regions are made up several token types. There are different restrictions and requirements for each type.

- `color` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
- `decoration` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
- `id` : Min 1, max 1. This token defines a value to associated with all `name` tokens that are present.
- `name` : Min 1. This token defines a name/tag pair.
    - This token type's value is structured as `[name] @[tag]`, where the `[name]` portion can be any non-blank string that does not contain an `@` character, and the `[tag]` portion can be any string that does not contain whitespace.
- `last-name` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.
    - This token type's value structure is identical to that of `name` tags.
- `pronoun` : Min 0, max 1. This token defines a value to associate with all `name` tokens that are present.

**NOTE:** All token values *should* be unique, even though Petrichor wont take issue with it. If a value is repeated, the AutoHotkey script generated from the input data will misbehave in unpredictable ways.

**Example:**

```petrichor
entry: // all optional tokens present
{
    id: 1234
    name: Sam @sm
    name: Sammy @smy
    last-name: Smith @s
    pronoun: they/them
    color: #89abcd
    decoration: -- a person
}

entry: // only required tokens present
{
    id: 4321
    name: ALEX @AX
}
```

---

### 5.1.2 - `generateTextShortcutScript` command

To generate a text hotstring shortcut script, call Petrichor with the command argument `generateTextShortcutScript`.

This command supports the following options:

- [--inputFile](#4321----inputfile-option-optional)
- [--outputFile](#4322----outputfile-option-optional)
- [--logMode](#4323----logmode-option-optional)
- [--logFile](#4324----logfile-option-optional)


**Example:**

```
{install path}\Petrichor\
>Petrichor.exe generateTextShortcutScript --inputFile {file} --outputFile {file} --logMode {mode} --logFile {file}
```

---

### 5.1.3 - Using the script generated by Petrichor

Petrichor text shortcut scripts are AutoHotkey scripts. Petrichor itself cannot run them.

---

#### 5.1.3.1 - Install AutoHotkey

Before you can do anything with your script, you need to install AutoHotkey. Download and install [AutoHotkey v2](https://www.autohotkey.com), then continue.

---

#### 5.1.3.2 - Running the script

Either double-click the .ahk file or right click on it and click "run script" in the dropdown menu.

---

#### 5.1.3.3 - Methods to launch the script automatically

If you get sick of launching a script manually, there are a few options.

---

##### 5.1.3.3.1 - Windows Startup shortcut (RECOMMENDED)

This is the simplest method. It's not totally reliable, but it works the majority of the time. Occasionally a script will launch successfully, but not show up in the taskbar tray. If that bothers you, just relaunch the script manually.

Here's how to do it:

1. Right-click the script in File Explorer.
2. Click `Create shortcut` in the dropdown menu.
3. Press `Win+R` to open the Windows Run dialog.
4. Type `shell:startup` into the dialog, then click OK.
5. The Startup folder will open. Copy the shortcut you created in step 2 into it.

---

##### 5.1.3.3.2 - Task Scheduler

Wi've found this method to be less reliable than the Windows Startup method, but it does work more often than not. It's also kind of a pain to set up. Wi recommend using the Windows Startup method over this one, unless that method doesnt work for you.

You can follow the directions [here](https://windowsloop.com/run-autohotkey-script-at-windows-startup/) to set it up.

---

##### 5.1.3.3.3 - Registry (NOT RECOMMENDED)

**DO NOT DO THIS UNLESS YOU KNOW WHAT YOU'RE DOING.**
**Editing the registry can brick your computer if you're not careful.**

Wi strongly recommend using one of the other methods above, unless all of them dont work for you.

Also, wi havent personally tested this method, so wi dont know how reliable it is, but it probably should work about the same as the other two?

1. Open the Registry Editor. There are two days to do this:
    - Press `Win+R` to open the Run dialog, type in `regedit`, then click OK.
    - Open the Start menu and search for either `regedit` or `Registry Editor`.
2. Navigate to `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`.
3. Add a new String key. Name it however you prefer.
4. Edit the value of the new string key and put in `"@:\path\to\autohotkey\version\file.exe" "@:\path\to\script\file.ahk"`, using the filepaths of your AutoHotkey installation and your script file.

---
