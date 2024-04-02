---
short_title: Petrichor Script
title: Petrichor Script - Text Shortcut Script Generation module
---

<h1 align="center">Petrichor Script</h1>
<h2 align="center"><a href="./index.html">Text Shortcut Script Generation module</a></h2>


---
## Module options token

???+ important "Restrictions"

	OPTIONAL

	Minumum required: 0

	Maximum allowed: 1

	Must come after `metadata` token.

This module's variant of the [module options token](../../getting-started/petrichor-script.html#module-options-token) supports the following tokens.

---
### Default icon and suspend icon tokens

**OPTIONAL**

These tokens allow you to specify file paths to custom icons for a text shortcut script.

The `default-icon` token sets the file path of the icon shown on a script by default.

The `suspend-icon` token sets the file path of the icon shown when a script is suspended.

[Relative file paths](../../getting-started/command-usage.html#relative-file-paths) can be used.

**IMPORTANT NOTE:** If you move an icon file and do not update its path in your input file, the icon will not be found and will not be applied.

???+ example

	```petrichor
	module-options:
	{
		default-icon: [path]/default.ico
		suspend-icon: ./[suspend].ico
	}
	```


---
### Reload shortcut and suspend shortcut tokens

**OPTIONAL**

These tokens allow you to set keyboard shortcuts to control the operation of a script.

The `reload-shortcut` token sets a keyboard shortcut to reload a script.

The `suspend-shortcut` token sets a keyboard shortcut to suspend and resume a script.

These keyboard shortcuts can be written in AutoHotkey v2 syntax, but for simplicity, Petrichor supports a [find-and-replace table](#control-shortcut-find-and-replace-table) of common modifier keys.

???+ example

	```petrichor
	module-options:
	{
		reload-shortcut: #r // Windows key + R
		suspend-shortcut: !s // Alt key + S
	}
	```


---
#### Control shortcut find-and-replace table

Petrichor supports the following tags in [script control shortcuts](#reload-shortcut-and-suspend-shortcut-tokens).

| Full tag        | Short tag | Encodes for |
| --------------- | --------- | ----------- |
| [windows]       | [win]     | Windows key
| [alt]           |           | Either Alt key
| [left-alt]      | [lalt]    | Left Alt key
| [right-alt]     | [ralt]    | Right Alt key
| [control]       | [ctrl]    | Either Control key
| [left-control]  | [lctrl]   | Left Control key
| [right-control] | [rctrl]   | Right Control key
| [shift]         |           | Either Shift key
| [left-shift]    | [lshift]  | Left Shift key
| [right-shift]   | [rshift]  | Right Shift key
| [and]           |           | &
| [alt-graph]     | [altgr]   | AltGr (AltGraph) key
| [wildcard]      | [wild]    | *
| [passthrough]   | [tilde]   | ~
| [send]          |           | $
| [tab]           |           | Tab key
| [caps-lock]     | [caps]    | CapsLock key
| [enter]         |           | Enter key
| [backspace]     | [bksp]    | Backspace key
| [insert]        | [ins]     | Insert key
| [delete]        | [del]     | Delete key
| [home]          |           | Home key
| [end]           |           | End key
| [page-up]       | [pgup]    | PageUp key
| [page-down]     | [pgdn]    | PageDown key
| \\[              |           | [
| \\]              |           | ]

!!! note

	`\[` and `\]` make use of [escape characters](../../getting-started/petrichor-script.html#escape-characters).

???+ example

	```petrichor
	module-options:
	{
		reload-shortcut: [win]r // Windows key + R
		suspend-shortcut: \[win\]s // [win]s
	}
	```


---
## Shortcut list token

**REQUIRED**

This token defines the text shortcuts to be generated.

Shortcuts are defined via subtokens within this token's body.


---
### Shortcut tokens

**OPTIONAL**

The `shortcut` token defines a plaintext shortcut.

These tokens' values can use AutoHotkey special behavior if written correctly. Consult AutoHotkey documentation to learn more about this.

Shortcuts consist of 3 parts: A hotstring, a divider consisting of 2 colons ( `::` ), and a replacement string.

**NOTE:** You cannot use `::` in a "find" string due to the way AutoHotkey hotstrings work. Petrichor will allow it, but the shortcuts it generates will not work.

These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by surrounding it with backticks `` ` ``.

???+ example

	```petrichor title="Input"
	shortcut-list:
	{
		shortcut: [hotstring] :: ` [replacement string] `
	}
	```
	```txt title="Shortcuts generated from input"
	::[hotstring]::` [replacement string] ` // This is a standard shortcut. The [hotstring] and [replacement string] will be inserted into the output file unaltered.
	```


---
### Shortcut template tokens

**OPTIONAL**

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

???+ example

	```petrichor title="Input"
	shortcut-list:
	{
		shortcut-template: [tag] [last-tag] :: [id] - [name] [last-name] ([pronoun]) | {[decoration]} | [color]
	}
	```
	```txt title="Shortcuts generated from input"
	::sm smt::1234 - Sam Smith (they/them) | {a person} | #123456
	::jo brn::5678 - Joe Brown (they/them) | {another person person} | #789abc
	```


---
#### Find and replace tokens

**OPTIONAL**

The `find` and `replace` token pair defines a custom find-and-replace dictionary for a `shortcut-template` token.

The find-and-replace dictionary is only applied to the template's replacement string.

It is applied after `[field]` tags are populated with data, and therefore can modify that data.

`Find keys` and `replace values` are defined in comma-separated lists surrounded by curly brackets ( `{` / `}` ).

The lists cannot contain blank items, and they must contain the same number of items as each other.

`replace` tokens must be paired with a matching `find` token, and must come after it.

`find` tokens can be present without a matching `replace` token. This will cause all the `find` keys to be simply removed from a template.

???+ example

	```petrichor title="Input"
	shortcut-list:
	{
		shortcut-template: [hotstring] :: [replacement string] custom find 1, custom find 2
		{
			find: { custom find 1, custom find 2 } // These are the `find keys`.
			replace: { replace 1, replace 2 } // These are the corresponding `replace values`.
		}
	}
	```
	```txt title="Shortcuts generated from input"
	::[hotstring]::[replacement string] replace 1, replace 2
	```
	If the `find keys` are present in `[field]` values within the `replacement string`, they will be replaced there as well.


---
## Text case tokens

The `text-case` token is used to change the text case of a shortcut after it is generated from a template.

Case conversion is applied after `[field]` tags are populated and [find-and-replace dictionaries](#find-and-replace-tokens) are applied.

Allowed values:

- unchanged (as-written; default)
- upper (UPPER CASE)
- lower (lower case)
- firstCaps (First Capitals Case)

???+ example

	=== "unchanged"

		```petrichor title="Input"
		shortcut-list:
		{
			shortcut-template: [hotstring] :: [replacement STRING]
			{
				text-case: unchanged
			}
		}
		```
		```txt title="Shortcuts generated from input"
		::[hotstring]::[replacement STRING]
		```

	=== "upper"

		```petrichor title="Input"
		shortcut-list:
		{
			shortcut-template: [hotstring] :: [replacement STRING]
			{
				text-case: upper
			}
		}
		```
		```txt title="Shortcuts generated from input"
		::[hotstring]::[REPLACEMENT STRING]
		```

	=== "lower"

		```petrichor title="Input"
		shortcut-list:
		{
			shortcut-template: [hotstring] :: [replacement STRING]
			{
				text-case: lower
			}
		}
		```
		```txt title="Shortcuts generated from input"
		::[hotstring]::[replacement string]
		```

	=== "firstCaps"

		```petrichor title="Input"
		shortcut-list:
		{
			shortcut-template: [hotstring] :: [replacement STRING]
			{
				text-case: firstCaps
			}
		}
		```
		```txt title="Shortcuts generated from input"
		::[hotstring]::[Replacement String]
		```


---
## Entry list token

**REQUIRED**

The `entry-list` token defines entries to populated templated shortcuts with.

This token's body contains subtokens defining each entry.

???+ example

	```petrichor
	entry-list:
	{
		// Entry subtokens go here.
	}
	```


---
### Entry tokens

**OPTIONAL**

The `entry` token defines a set of data to populate a templated shortcut with.

This token's value is ignored, but will show up in logs. You can put notes into it if desired.

This token's body contains subtokens defining its data.

These subtokens correspond to the `[field]` tags in [templated shortcuts](#shortcut-template-tokens).

**NOTE:** All token values *should* be unique, even though Petrichor wont take issue with it. If a value is repeated, the AutoHotkey script generated from the input data will misbehave in unpredictable ways.

???+ example

	```petrichor
	entry: Notes about entry.
	{
		// Entry data subtokens go here.
	}
	```


---
#### Color tokens

**OPTIONAL**

The `color` token defines a value for the `[color]` field tag in [templated shortcuts](#shortcut-template-tokens).

At most 1 can be present in an `entry` token's body.


---
#### Decoration tokens

**OPTIONAL**

The `decoration` token defines a value for the `[decoration]` field tag in [templated shortcuts](#shortcut-template-tokens).

At most 1 can be present in an `entry` token's body.


---
#### ID tokens

**REQUIRED**

The `id` token defines a value for the `[id]` field tag in [templated shortcuts](#shortcut-template-tokens).

Exactly 1 must be present in an `entry` token's body.


---
#### Name tokens

**REQUIRED**

The `name` token defines values for the `[name]` and `[tag]` field tags in [templated shortcuts](#shortcut-template-tokens).

At least 1 must be present in an `entry` token's body.

A shortcut will be generated from each template for each `name` token in an entry.

`name` token values must consist of a `[name]` field and a `[tag]` field, separated by an at-sign ( `@` ).

The `[name]` field value can be any non-blank string that does not contain an at-sign ( `@` ).

The `[tag]` field value can be any string that does not contain whitespace.


???+ example

	```petrichor
	entry:
	{
		name: name string @tagstring
	}
	```


---
#### Last name tokens

**OPTIONAL**

The `last-name` token defines values for the `[last-name]` and `[last-tag]` field tags in [templated shortcuts](#shortcut-template-tokens).

At most 1 can be present in an `entry` token's body.

`last-name` tokens have the same structural requirements as [`name` tokens](#name-tokens).


---
#### Pronoun tokens

**OPTIONAL**

The `pronoun` token defines a value for the `[pronoun]` field tag in [templated shortcuts](#shortcut-template-tokens).

At most 1 can be present in an `entry` token's body.

---
### Example of all module tokens

???+ example

	```petrichor title="Input"
	metadata:
	{
		// Metadata tokens go here.
	}

	module-options:
	{
		default-icon: [file path].ico
		suspend-icon: [file path].ico
		reload-shortcut: [shortcut]
		suspend-shortcut: [shortcut]
	}

	shortcut-list:
	{
		shortcut: replaceme :: withme
		shortcut-template: [tag] :: [name] [last-name] ([pronoun])
		{
			find: { this, that }
			replace: { these, those }
			text-case: firstCaps
		}
	}

	entry-list:
	{
		entry: A
		{
			id: idValueA
			color: colorValueA
			decoration: decorationValueA
			pronoun: pronounValueA
			last-name: last name value A @lastTagValueA
			name: name value A 1 @tagValueA1
			name: name value A 2 @tagValueA2
		}

		entry: B
		{
			id: idValueB
			color: colorValueB
			decoration: decorationValueB
			pronoun: pronounValueB
			last-name: last name value B @lastTagValueB
			name: name value B 1 @tagValueB1
			name: name value B 2 @tagValueB2
		}
	}
	```
	```txt title="Shortcuts generated from input"
	::replaceme::withme
	::tagValueA1::Name Value A 1 Last Name Value A (Pronounvaluea)
	::tagValueA2::Name Value A 2 Last Name Value A (Pronounvaluea)
	::tagValueB1::Name Value B 1 Last Name Value B (Pronounvalueb)
	::tagValueB2::Name Value B 2 Last Name Value B (Pronounvalueb)
	```
