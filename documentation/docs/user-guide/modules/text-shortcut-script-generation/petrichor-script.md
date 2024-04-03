---
short_title: Petrichor Script
title: Petrichor Script - Text Shortcut Script Generation module
---

<h1 align="center">Petrichor Script</h1>
<h2 align="center"><a href="./index.html">Text Shortcut Script Generation module</a></h2>


---
## Module options token

This module's variant of the [module options token](../../getting-started/petrichor-script.html#module-options-token) supports the following tokens.

- [Default icon](#default-icon-and-suspend-icon-tokens)
- [Suspend icon](#default-icon-and-suspend-icon-tokens)
- [Reload shortcut](#reload-shortcut-and-suspend-shortcut-tokens)
- [Suspend shortcut](#reload-shortcut-and-suspend-shortcut-tokens)

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must come after `metadata` token.

???+ example

    ```petrichor
    module-options:
    {
        // Module-specific options go here.
    }
    ```

---
### Default icon and suspend icon tokens

These tokens allow you to specify file paths to custom icons for a text shortcut script.

The `default-icon` token sets the file path of the icon shown on a script by default.

The `suspend-icon` token sets the file path of the icon shown when a script is suspended.

[Relative file paths](../../getting-started/command-usage.html#relative-file-paths) can be used.

!!! warning

    If you move an icon file and do not update its path in your input file, the icon will not be found and will not be applied.

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `module-options` token body.

???+ example

    ```petrichor
    module-options:
    {
        default-icon: <path>/default.ico
        suspend-icon: ./suspend_icon.png
    }
    ```


---
### Reload shortcut and suspend shortcut tokens

These tokens allow you to set keyboard shortcuts to control the operation of a script.

The `reload-shortcut` token sets a keyboard shortcut to reload a script.

The `suspend-shortcut` token sets a keyboard shortcut to suspend and resume a script.

These keyboard shortcuts can be written in AutoHotkey v2 syntax, but for simplicity, Petrichor supports a [find-and-replace table](#control-shortcut-find-and-replace-table) of common modifier keys.

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `module-options` token body.

???+ example

    ```petrichor
    module-options:
    {
        reload-shortcut: <shortcut>
        suspend-shortcut: <shortcut>
    }
    ```


---
#### Control shortcut find-and-replace table

Petrichor supports the following tags in [script control shortcuts](#reload-shortcut-and-suspend-shortcut-tokens).

| Tag                      | Alias              | Encodes for       | Key / symbol name                     |
| ------------------------ | ------------------ | ----------------- | ------------------------------------- |
| `#!ptcr [windows]`       | `#!ptcr [win]`     | ++win++           | Windows key                           |
| `#!ptcr [alt]`           |                    | ++alt++           | Alt key                               |
| `#!ptcr [left-alt]`      | `#!ptcr [lalt]`    | ++lalt++          | Left Alt key                          |
| `#!ptcr [right-alt]`     | `#!ptcr [ralt]`    | ++ralt++          | Right Alt key                         |
| `#!ptcr [control]`       | `#!ptcr [ctrl]`    | ++ctrl++          | Control key                           |
| `#!ptcr [left-control]`  | `#!ptcr [lctrl]`   | ++lctrl++         | Left Control key                      |
| `#!ptcr [right-control]` | `#!ptcr [rctrl]`   | ++rctrl++         | Right Control key                     |
| `#!ptcr [shift]`         |                    | ++shift++         | Shift key                             |
| `#!ptcr [left-shift]`    | `#!ptcr [lshift]`  | ++lshift++        | Left Shift key                        |
| `#!ptcr [right-shift]`   | `#!ptcr [rshift]`  | ++rshift++        | Right Shift key                       |
| `#!ptcr [and]`           |                    | ++"&"++           | AutoHotkey combine (Ampersand symbol) |
| `#!ptcr [alt-graph]`     | `#!ptcr [altgr]`   | ++altgr++         | AltGraph key                          |
| `#!ptcr [wildcard]`      | `#!ptcr [wild]`    | ++"*"++           | AutoHotkey wildcard (Asterisk symbol) |
| `#!ptcr [passthrough]`   | `#!ptcr [tilde]`   | ++tilde++         | AutoHotkey passthrough (Tilde symbol) |
| `#!ptcr [send]`          |                    | ++"$"++           | AutoHotkey send (Dollar sign)         |
| `#!ptcr [tab]`           |                    | ++tab++           | Tab key                               |
| `#!ptcr [caps-lock]`     | `#!ptcr [caps]`    | ++caps-lock++     | CapsLock key                          |
| `#!ptcr [enter]`         |                    | ++enter++         | Enter key                             |
| `#!ptcr [backspace]`     | `#!ptcr [bksp]`    | ++backspace++     | Backspace key                         |
| `#!ptcr [insert]`        | `#!ptcr [ins]`     | ++ins++           | Insert key                            |
| `#!ptcr [delete]`        | `#!ptcr [del]`     | ++del++           | Delete key                            |
| `#!ptcr [end]`           |                    | ++end++           | End key                               |
| `#!ptcr [home]`          |                    | ++home++          | Home key                              |
| `#!ptcr [page-up]`       | `#!ptcr [pgup]`    | ++page-up++       | PageUp key                            |
| `#!ptcr [page-down]`     | `#!ptcr [pgdn]`    | ++page-dn++       | PageDown key                          |
| `#!ptcr \[`              |                    | ++bracket-left++  | Left square bracket                   |
| `#!ptcr \]`              |                    | ++bracket-right++ | Right square bracket                  |

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

This token defines the text shortcuts to be generated.

Shortcuts are defined via subtokens within this token's body.

???+ important "Restrictions"

    REQUIRED

    Mininum required: 1

    Maximum allowed: 1

    Must come after `metadata` token.

???+ example

    ```petrichor
    shortcut-list:
    {
        // Shortcuts go here.
    }
    ```


---
### Shortcut tokens

The `shortcut` token defines a plaintext shortcut.

These tokens' values can use AutoHotkey special behavior if written correctly. Consult AutoHotkey documentation to learn more about this.

Shortcuts consist of 3 parts: A hotstring, a divider consisting of 2 colons ( `::` ), and a replacement string.

These components can have whitespace between them, but note that this whitespace will be trimmed off unless you force it to be kept in by surrounding it with backticks `` ` ``.

!!! warning

    You cannot use `::` in a hotstring due to the way AutoHotkey hotstrings work.

    Petrichor will allow you to do it, but the generated shortcuts will not work.

???+ important "Restrictions"

    OPTIONAL

    No restrictions on number of instances that can be present.

    Must be in `shortcut-list` token body.

???+ example

    ```petrichor title="Input"
    shortcut-list:
    {
        shortcut: <hotstring> :: ` <replacement string> `
    }
    ```
    ```autohotkey title="Shortcuts generated from input"
    ; This is a standard shortcut. The <hotstring> and <replacement string> will be inserted into the output file unaltered.
    ::<hotstring>::` <replacement string> `
    ```


---
### Shortcut template tokens

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

Additional features are supported via subtokens.

If no subtokens are used, this token does not need a body.

!!! note

    By default, you cannot use the `[` or `]` symbols in a template. Use [escape characters](../../getting-started/command-usage.html#escape-characters) to circumvent this.

!!! warning

    You cannot use `::` in a hotstring due to the way AutoHotkey hotstrings work.

    Petrichor will allow you to do it, but the generated shortcuts will not work.

???+ important "Restrictions"

    OPTIONAL

    No restrictions on number of instances that can be present.

    Must be in `shortcut-list` token body.

???+ example

    ```petrichor title="Input"
    shortcut-list:
    {
        shortcut-template: [tag] [last-tag] :: [id] - [name] [last-name] ([pronoun]) | {[decoration]} | [color]
    }
    ```
    ```autohotkey title="Shortcuts generated from input"
    ::sm smt::1234 - Sam Smith (they/them) | {a person} | #123456
    ::jo brn::5678 - Joe Brown (they/them) | {another person person} | #789abc
    ```


---
#### Find and replace tokens

The `find` and `replace` token pair defines a custom find-and-replace dictionary for a `shortcut-template` token.

The find-and-replace dictionary is only applied to the template's replacement string.

It is applied after `[field]` tags are populated with data, and therefore can modify that data.

`Find keys` and `replace values` are defined in comma-separated lists surrounded by curly brackets ( `{` / `}` ).

The `find` and `replace` lists cannot contain blank items.

The `find` and `replace` lists must contain the same number of items as each other.

!!! note

    `Find keys` are case-sensitive.

!!! note

    If a `find` token does not have a matching `replace` token, all the `find` keys will just be removed from the template.

???+ important "Find token restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `shortcut-template` token body.


???+ important "Replace token restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `shortcut-template` token body.

    Must be paired with `find` token.

    Must come after `find` token.


???+ example

    ```petrichor title="Input"
    shortcut-list:
    {
        shortcut-template: <hotstring> :: <replacement string> custom find 1, custom find 2, Custom find 2
        {
            find: { custom find 1, custom find 2 } // These are the `find keys`.
            replace: { replace 1, replace 2 } // These are the corresponding `replace values`.
        }
    }
    ```
    ```autohotkey title="Shortcuts generated from input"
    ::<hotstring>::<replacement string> replace 1, replace 2, Custom find 2
    ```
    If the `find keys` are present in `[field]` values within the `<replacement string>`, they will be replaced there as well.


---
## Text case tokens

The `text-case` token is used to change the text case of a shortcut after it is generated from a template.

Case conversion is applied after `[field]` tags are populated and [find-and-replace dictionaries](#find-and-replace-tokens) are applied.

Allowed values:

- unchanged (as-written; default)
- upper (UPPER CASE)
- lower (lower case)
- firstCaps (First Capitals Case)

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `shortcut-template` token body.

???+ example

    === "unchanged"

        ```petrichor title="Input"
        shortcut-list:
        {
            shortcut-template: <hotstring> :: <replacement STRING>
            {
                text-case: unchanged
            }
        }
        ```
        ```autohotkey title="Shortcuts generated from input"
        ::<hotstring>::<replacement STRING>
        ```

    === "upper"

        ```petrichor title="Input"
        shortcut-list:
        {
            shortcut-template: <hotstring> :: <replacement STRING>
            {
                text-case: upper
            }
        }
        ```
        ```autohotkey title="Shortcuts generated from input"
        ::<hotstring>::<REPLACEMENT STRING>
        ```

    === "lower"

        ```petrichor title="Input"
        shortcut-list:
        {
            shortcut-template: <hotstring> :: <replacement STRING>
            {
                text-case: lower
            }
        }
        ```
        ```autohotkey title="Shortcuts generated from input"
        ::<hotstring>::<replacement string>
        ```

    === "firstCaps"

        ```petrichor title="Input"
        shortcut-list:
        {
            shortcut-template: <hotstring> :: <replacement STRING>
            {
                text-case: firstCaps
            }
        }
        ```
        ```autohotkey title="Shortcuts generated from input"
        ::<hotstring>::<Replacement String>
        ```


---
## Entry list token

The `entry-list` token defines entries to populated templated shortcuts with.

This token's body contains subtokens defining each entry.

???+ important "Restrictions"

    REQUIRED

    Minimum required: 1

    Maximum allowed: 1

    Must come after `metadata` token.

???+ example

    ```petrichor
    entry-list:
    {
        // Entries go here.
    }
    ```


---
### Entry tokens

The `entry` token defines a set of data to populate a templated shortcut with.

This token's value is ignored, but will show up in logs. You can put notes into it if desired.

This token's body contains subtokens defining its data.

These subtokens correspond to the `[field]` tags in [templated shortcuts](#shortcut-template-tokens).

**NOTE:** All token values *should* be unique, even though Petrichor wont take issue with it. If a value is repeated, the AutoHotkey script generated from the input data will misbehave in unpredictable ways.

???+ important "Restrictions"

    OPTIONAL

    No restrictions on number of instances that can be present.

    Must be in `entry-list` token body.

???+ example

    ```petrichor
    entry: Notes about entry.
    {
        // Entry data goes here.
    }
    ```


---
#### Color tokens

The `color` token defines a value for the `[color]` field tag in [templated shortcuts](#shortcut-template-tokens).

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `entry` token body.

---
#### Decoration tokens

The `decoration` token defines a value for the `[decoration]` field tag in [templated shortcuts](#shortcut-template-tokens).

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `entry` token body.


---
#### ID tokens

The `id` token defines a value for the `[id]` field tag in [templated shortcuts](#shortcut-template-tokens).

???+ important "Restrictions"

    REQUIRED

    Minimum required: 1

    Maximum allowed: 1

    Must be in `entry` token body.


---
#### Name tokens

The `name` token defines values for the `[name]` and `[tag]` field tags in [templated shortcuts](#shortcut-template-tokens).

A shortcut will be generated from each template for each `name` token in an entry.

`name` token values must consist of a `[name]` field and a `[tag]` field, separated by an at-sign ( `@` ).

The `[name]` field value can be any non-blank string that does not contain an at-sign ( `@` ).

The `[tag]` field value can be any string that does not contain whitespace.

???+ important "Restrictions"

    REQUIRED

    Minimum required: 1

    Must be in `entry` token body.

???+ example

    ```petrichor
    entry:
    {
        name: name string @tagstring
    }
    ```


---
#### Last name tokens

The `last-name` token defines values for the `[last-name]` and `[last-tag]` field tags in [templated shortcuts](#shortcut-template-tokens).

`last-name` tokens have the same structure as [`name` tokens](#name-tokens).

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `entry` token body.


---
#### Pronoun tokens

The `pronoun` token defines a value for the `[pronoun]` field tag in [templated shortcuts](#shortcut-template-tokens).

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in `entry` token body.


---
### Full usage example

???+ example

    ```petrichor title="Input"
    metadata:
    {
        // Metadata goes here.
    }


    module-options:
    {
        default-icon: <file path>
        suspend-icon: <file path>
        reload-shortcut: <shortcut>
        suspend-shortcut: <shortcut>
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
            pronoun: pronounValueA THIS THAT
            last-name: last name value A @lastTagValueA
            name: name value A 1 @tagValueA1
            name: name value A 2 @tagValueA2
        }

        entry: B
        {
            id: idValueB
            color: colorValueB
            decoration: decorationValueB
            pronoun: pronounValueB this that
            last-name: last name value B @lastTagValueB
            name: name value B 1 @tagValueB1
            name: name value B 2 @tagValueB2
        }
    }
    ```
    ```autohotkey title="Shortcuts generated from input"
    ::replaceme::withme
    ::tagValueA1::Name Value A 1 Last Name Value A (Pronounvaluea This That)
    ::tagValueA2::Name Value A 2 Last Name Value A (Pronounvaluea This That)
    ::tagValueB1::Name Value B 1 Last Name Value B (Pronounvalueb These Those)
    ::tagValueB2::Name Value B 2 Last Name Value B (Pronounvalueb These Those)
    ```
