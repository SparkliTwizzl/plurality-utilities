---
short_title: Petrichor Script
title: Petrichor Script - Getting started
---

<h1 align="center">Petrichor Script</h1>
<h2 align="center"><a href="./index.html">Getting started</a></h2>


## Syntax

Petrichor Script is made up of data tokens, which may be nested.

Petrichor Script files use the extension `.petrichor` or `.ptcr`.

---
### Data tokens

All data in Petrichor input files is in the form of data tokens, or just "tokens".

Some tokens are required and some are optional.

Every token consists of a name and a value, separated by a colon ( `:` ).

Token names are in `lower-kebab-case`.

Token names can contain numbers, but cannot start with them.

!!! note

    Whitespace around token names and values is ignored.

???+ example

    ```petrichor title="Valid token names"
    tokenname: Value.
    token-name: Value.
    token-name-3: Value.
    ```
    
    ```petrichor title="Invalid token names"
    TokenName: Value.
    token_name: Value.
    3-token-name: Value.
    ```

    ```petrichor title="Whitespace"
    // These tokens are identical to Petrichor.
     token-name : Token value. 
    token-name:Token value.
    
    // This token is different than the first two.
    token-name:Token   value.
    ```

---
#### Token bodies

Some tokens can have a body containing other tokens.

These consist of a token (which may or may not require a value), then the body, which is surrounded by curly brackets ( `{` `}` ).

Tokens within a token body can also have bodies.

The contents of the token's body can be indented for readability if desired, but it is not required.

???+ example

    ```petrichor
    parent-token-name:
    {
        child-token-1-name: Value.
        {
            child-token-2-name: Value.
        }
    }
    ```

---
### Escape characters

Backslash ( `\` ) is treated as an "escape character".

It is used to disable (aka "escape") the normal function of special characters and make them be treated as literal text instead.

!!! tip

    You can escape an escape character.

???+ example

    ```petrichor
    // This example token treats @ as a special character and performs operations on it.
    do-something: @to-this

    // In this case, the @ will be treated as literal text and no operations will be performed on it.
    do-something: \@but-not-to-this

    // Here, the escape character is escaped and treated as a literal backslash ( \ ) character.
    // The @ is not escaped and will be treated as a special character like normal.
    do-something: \\@to-this-too
    ```

---
### Blank lines and comments

Blank lines are ignored.

The sequence `//` starts a comment which continues to the end of the line and will be ignored.

Comments can be [escaped](#escape-characters) to make Petrichor treat them as regular text.

???+ example

    ```petrichor
    // This is a comment. This line will be ignored.
    // The following line is blank, and will also be ignored.

    token: value // This is an inline comment. Everything after "//" will be ignored.
    token: value \// This is part of the value. // But this is a comment and will be ignored.
    ```

---
## Universal tokens

These tokens are universal to all modules.

Modules may support different (non-universal) contents within a universal token's body.

Consult a module's documentation for more information.

---
### Metadata token

The `metadata` token's body contains information necessary for Petrichor to run.

???+ important "Restrictions"

    REQUIRED

    Minimum required: 1

    Maximum allowed: 1

    Must be first token in input file.

???+ example

    ```petrichor
    metadata:
    {
        // Metadata goes here.
    }

    // All other tokens go here.
    ```

---
#### Minimum version token

The `minimum-version` token specifies the minimum Petrichor version required in order to parse the file.

Version numbers are in the format `major.minor.patch.preview`.

`Major` and `minor` version must be specified.

!!! info

    If `patch` version or `patch` and `preview` versions are blank, they are assumed to be any version.

!!! warning

    The `patch` version cannot be blank if the `preview` version is specified.

!!! tip

    Set this token's value to the minimum Petrichor version required to handle your input file.

???+ important "Restrictions"

    REQUIRED

    Minimum required: 1

    Maximum allowed: 1

    Must be in [`metadata`](#metadata-token) token body.

???+ example

    ```petrichor title="Major version 1, minor version 2, patch version 3, preview version pre-4"
    metadata:
    {
        minimum-version: 1.2.3.pre-4
    }
    ```

    ```petrichor title="Major version 1, minor version 2, patch version 3, any preview version"
    metadata:
    {
        minimum-version: 1.2.3
    }
    ```

    ```petrichor title="Major version 1, minor version 2, any patch or preview version"
    metadata:
    {
        minimum-version: 1.2
    }
    ```

    ```petrichor title="(NOT ALLOWED) Major version 1, minor version 2, blank patch version, preview version pre-4"
    metadata:
    {
        minimum-version: 1.2..pre-4 // This is not allowed by Petrichor.
    }
    ```

---
#### Command token

The `command` token allows you to specify the [command to run](command-usage.md) in your input file, and Petrichor will handle it automatically when run with the input file.

Set the token's value to the name of the command to be run.

To use command options, add a body to the token and put subtokens into it, converting the command options into [token names](#data-tokens) and setting the tokens' values to the command option arguments.

!!! tip

    Setting module commands with this token may be easier than command line arguments if you are not comfortable with using terminals.

!!! tip

    Using this token allows you to run Petrichor by simply dragging-and-dropping the input file onto `Petrichor.exe`.

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must be in [`metadata`](#metadata-token) token body.

???+ example

    ```petrichor title="Input"
    metadata:
    {
        minimum-version: <version number>
        command: commandName
        {
            command-option-1: "argument 1"
            command-option-2: "argument 2"
        }
    }
    ```
    ```powershell title="Command line"
    [install path]> Petrichor.exe input.txt
    ```

    This is equivalent to the following:
    ```petrichor title="Input"
    metadata:
    {
        minimum-version: <version number>
    }
    ```
    ```powershell title="Command line"
    [install path]> Petrichor.exe commandName --inputFile input.txt --commandOption1 "argument 1" --commandOption2 "argument 2"
    ```

---
### Module options token

The `module-options` token allows you to configure module-specific options, if supported by a module.

!!! note

    Each module that supports this token will have its own version of it.
    
    See the relevant module's documentation for more information.

???+ important "Restrictions"

    OPTIONAL

    Maximum allowed: 1

    Must come after [`metadata`](#metadata-token) token.
    
???+ example

    ```petrichor
    metadata:
    {
        // Metadata goes here.
    }

    module-options:
    {
        // Supported tokens depend on module in use.
    }
    ```

---
### Module-specific tokens

Modules have unique tokens that are specific to their functions.

See the relevant module's documentation for information about its tokens.
