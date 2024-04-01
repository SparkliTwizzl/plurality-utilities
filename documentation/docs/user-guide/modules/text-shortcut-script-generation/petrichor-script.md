---
short_title: Petrichor Script
title: Petrichor Script - Text Shortcut Script Generation module
---

<h1 align="center">Petrichor Script</h1>
<h2 align="center"><a href="./index.html">Text Shortcut Script Generation module</a></h2>


---
## Module options token

(OPTIONAL)

This module's variant of the [module options token](../../getting-started/petrichor-script.html#module-options-token) supports the following tokens.

---
### Default icon and suspend icon tokens

(OPTIONAL)

These tokens allow you to specify file paths to custom icons for a text shortcut script.

The `default-icon` token sets the file path of the icon shown on a script by default.

The `suspend-icon` token sets the file path of the icon shown when a script is suspended.

[Relative file paths](../../getting-started/command-usage.html#relative-file-paths) can be used.

**IMPORTANT NOTE:** If you move an icon file and do not update its path in your input file, the icon will not be found and will not be applied.

**Example:**
```petrichor
module-options:
{
    default-icon: [path]/default.ico
    suspend-icon: ./[suspend].ico
}
```


---
### Reload shortcut and suspend shortcut tokens

(OPTIONAL)

These tokens allow you to set keyboard shortcuts to reload or suspend a text shortcut script.

The `reload-shortcut` token sets a keyboard shortcut to reload a script.

The `suspend-shortcut` token sets a keyboard shortcut to suspend and resume a script.

These keyboard shortcuts can be written in AutoHotkey v2 syntax, but for simplicity, Petrichor supports a [find-and-replace table](#control-shortcut-find-and-replace-table) of common modifier keys.

**Example:**
```petrichor
module-options:
{
    reload-shortcut: #r // Windows key + R
    suspend-shortcut: !s // Alt key + S
}
```

---
#### Control shortcut find-and-replace table

Petrichor supports these tags in [script control shortcuts](#reload-shortcut-and-suspend-shortcut-tokens):

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
- `\[` → `[` (see [escape characters](../../getting-started/petrichor-script.html#escape-characters))
- `\]` → `]` (see [escape characters](../../getting-started/petrichor-script.html#escape-characters))

**Example:**
```petrichor
module-options:
{
    reload-shortcut: [win]r // Windows key + R
    suspend-shortcut: \[win\]s // [win]s
}
```


---
## Shortcut list token

(REQUIRED)

This token defines the text shortcuts to be generated.

Shortcuts are defined via subtokens within this token's body.


---
### Shortcut tokens

(OPTIONAL)

The `shortcut` token defines a plaintext shortcut.

These tokens' values can use AutoHotkey special behavior if written correctly. Consult AutoHotkey documentation to learn more about this.

Shortcuts consist of 3 parts: A hotstring, a divider consisting of 2 colons ( `::` ), and a replacement string.

**NOTE:** You cannot use `::` in a "find" string due to the way AutoHotkey hotstrings work. Petrichor will allow it, but the shortcuts it generates will not work.

These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by surrounding it with backticks `` ` ``.

**Example:**
```petrichor
shortcut-list:
{
    shortcut: [hotstring] :: ` [replacement string] `
}
```

SHORTCUTS GENERATED FROM INPUT:
```txt
::[hotstring]::` [replacement string] ` // This is a standard shortcut. The [hotstring] and [replacement string] will be inserted into the output file unaltered.
```


---
### Shortcut template tokens

(OPTIONAL)

The `shortcut-template` token defines a templated shortcut.

It behaves the same way as the [shortcut token](#shortcut-tokens), but with additional features.

Shortcuts will be generated from the template, filling in `[field]` tags with user-provided data.

Supported fields:

- `[color]`
- `[decoration]`
- `[id]`
- `[name]`
- `[last-name]`
- `[last-tag]`
- `[pronoun]`
- `[tag]`

**NOTE:** By default, you cannot use the `[` or `]` symbols in a template string. Use [escape characters](../../getting-started/command-usage.html#escape-characters) to circumvent this.

Additional features are supported via subtokens.

If no subtokens are used, this token does not need a body.

**Example:**

```petrichor
shortcut-list:
{
    shortcut-template: [tag] [last-tag] :: [id] - [name] [last-name] ([pronoun]) | {[decoration]} | [color]
}
```

SHORTCUTS GENERATED FROM INPUT:
```txt
::sm smt::1234 - Sam Smith (they/them) | {a person} | #123456
::jo brn::5678 - Joe Brown (they/them) | {another person person} | #789abc
```


---
#### Find and replace tokens

(OPTIONAL)

The `find` and `replace` token pair defines a custom find-and-replace dictionary for a `shortcut-template` token.

The find-and-replace dictionary is only applied to the template's replacement string.

It is applied after `[feild]` tags are populated with data, and therefore can modify that data.

`Find` keys and `replace` values are defined in comma-separated lists surrounded by curly brackets ( `{` / `}` ).

The lists cannot contain blank items, and they must contain the same number of items as each other.

`rReplace` tokens must be paired with a matching `find` token, and must come after it.

`find` tokens can be present without a matching `replace` token. This will cause all the `find` keys to be simply removed from a template.

**Example:**
```petrichor
shortcut-list:
{
    shortcut-template: [hotstring] :: [replacement string] custom find 1, custom find 2
    {
        find: { custom find 1, custom find 2 } // These are the `find` keys.
        replace: { replace 1, replace 2 } // These are the corresponding `replace` values.
    }
}
```

SHORTCUTS GENERATED FROM INPUT:
```txt
::[hotstring]::[replacement string] replace 1, replace 2 // If the `find` keys are present in `[field]` values within the [replacement string], they will be replaced there as well.
```


---
## Text case tokens

The `text-case` token is used to change the text case of a shortcut after it is generated from a template.

Case conversion is applied after `[field]` tags are populated and [find-and-replace dictionaries](#find-and-replace-tokens) are applied.

Allowed values:

- unchanged (as-written; default)
- upper (UPPER CASE)
- lower (lower case)
- firstCaps (First Capitals Case)

**Example:**
```petrichor
shortcut-list:
{
    shortcut-template: [hotstring] :: [replacement STRING]
    {
        text-case: unchanged
    }
    shortcut-template: [hotstring] :: [replacement STRING]
    {
        text-case: upper
    }
    shortcut-template: [hotstring] :: [replacement STRING]
    {
        text-case: lower
    }
    shortcut-template: [hotstring] :: [replacement STRING]
    {
        text-case: firstCaps
    }
}
```

SHORTCUTS GENERATED FROM INPUT:
```txt
::[hotstring]::[replacement STRING]
::[hotstring]::[REPLACEMENT STRING]
::[hotstring]::[replacement string]
::[hotstring]::[Replacement String]
```


---
## Entry list token

(REQUIRED)

The `entry-list` token defines entries to populated templated shortcuts with.

This token's body contains subtokens defining each entry.

**Example:**
```petrichor
entry-list:
{
	// Entry subtokens go here.
}
```


---
### Entry tokens

(OPTIONAL)

The `entry` token defines a set of data to populate a templated shortcut with.

This token's body contains subtokens defining its data.

**NOTE:** All token values *should* be unique, even though Petrichor wont take issue with it. If a value is repeated, the AutoHotkey script generated from the input data will misbehave in unpredictable ways.

**Example:**
```petrichor
entry: You can put notes in the value if you want to.
{
	// Entry data subtokens go here.
}
```


---
#### Color tokens

(OPTIONAL)

The `color` token defines a value in an `entry` token.

At most 1 can be present in an `entry` token's body.


---
#### Decoration tokens

(OPTIONAL)

The `decoration` token defines a value in an `entry` token.

At most 1 can be present in an `entry` token's body.


---
#### ID tokens

(REQUIRED)

The `id` token defines a value in an `entry` token.

Exactly 1 must be present in an `entry` token's body.


---
#### Name tokens

(REQUIRED)

The `name` token defines a pair of values in an `entry` token.

At least 1 must be present in an `entry` token's body.

`name` token values must consist of a `[name]` field and a `[tag]` field, separated by an at-sign ( `@` ).

The `[name]` field value can be any non-blank string that does not contain an at-sign ( `@` ).

The `[tag]` field value can be any string that does not contain whitespace.


**Example:**
```petrichor
entry:
{
	name: name string @tagstring
}
```


---
#### Last name tokens

(OPTIONAL)

The `last-name` token defines a pair of values in an `entry` token.

At most 1 can be present in an `entry` token's body.

`last-name` tokens have the same structural requirements as [`name` tokens](#name-tokens).


---
#### Pronoun tokens

(OPTIONAL)

The `pronoun` token defines a value in an `entry` token.

At most 1 can be present in an `entry` token's body.

---
### Full entry token example

```petrichor
entry:
{
	id: value
	color: value
	decoration: value
	pronoun: value
	last-name: name string @tagstring
	name: name string @tagstring
	name: name string @tagstring
}
```
