<h1 align="center">Petrichor Script</h1>


# Syntax

Petrichor Script is made up of data tokens, which may be nested.

Petrichor Script files use the extension `.petrichor` or `.ptcr`.

---
## Data tokens

All data in Petrichor input files is in the form of data tokens, or just "tokens".

Some tokens are required and some are optional.

Some tokens can have a body containing subtokens.

---
### Data token structure

Every token consists of a name and a value, separated by a color ( `:` ).

Whitespace between and around these parts is ignored.

Whitespace within these parts matters, however.

Token names are always in `lower-kebab-case`.

**Example (The first two tokens are identical to Petrichor, the third is different.):**

```petrichor
 token-name : Token value. 
token-name:Token value.
token-name:Token   value.
```

---
### Token bodies

Some tokens can have a body containing subtokens.

These consist of a token (which may or may not require a value), then the body, which is surrounded by curly brackets ( `{` `}` ).

Tokens within a token body can also have bodies.

The contents of the token's body can be indented for readability if desired, but it is not required.

**Example:**

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
## Blank lines and comments

Blank lines are ignored.

The sequence `//` starts a comment which continues to the end of the line and will be ignored.

Comments can be [escaped](#escape-characters) to make Petrichor treat them as regular text.

**Example:**

```petrichor
// This is a comment. This line will be ignored. The following line is blank, and will also be ignored.

token: value // This is an inline comment. Everything after "//" will be ignored.
token: value \// This is an escaped comment and is part of the value. // But this is a non-escaped comment and will be ignored.
```

---
## Escape characters

Backslash `\` is treated as an "escape character" in some cases. It is used to disable the normal function of special characters. An escape character can be applied to another escape character in order to make the scond one print literally.

**Example:**

```petrichor
do-something: @to-this // This example token treats @ as a special character and performs operations on it.
do-something: \@but-not-to-this // In this case, the @ will be treated as literal text and no operations will be performed on it.
do-something: \\@to-this-too // Here, the escape character is escaped, so the @ is not escaped and will be treated as a special character.
```

---
# Supported tokens

These tokens are universal to all input files.

Individual modules use non-universal tokens. Consult a module's documentation to see the tokens it supports.

---
## Metadata token

(REQUIRED)

The `metadata` token's body contains information necessary for Petrichor to run.

It must be the first token in the file regardless of what module is used.

Its value will be ignored.

---
### Minimum version token

(REQUIRED)

The `minimum-version` token specifies the minimum Petrichor version required in order to parse the file.

Version numbers are in the format `major.minor.patch.preview`.

Major and minor version must be specified.

If patch or patch and preview versions are blank, they are assumed to be any version.

**Example:**

```petrichor
minimum-version: 1.2.3.pre-4 // Major version 1, minor version 2, patch version 3, preview version pre-4.
minimum-version: 1.2.3 // Major version 1, minor version 2, patch version 3, any preview version.
minimum-version: 1.2 // Major version 1, minor version 2, any patch or preview version.
```

---
### Command token

(OPTIONAL)

The `command` token allows you to specify the [command to run](command-syntax.md) in your input file, and Petrichor will handle it automatically when run with the input file.

Set the token's value to the name of the command to be run.

To use command options, add a body to the token and put subtokens into it, converting the command options' names to `kebab-case` and setting the tokens' values to the command option values.

**Example:**

```petrichor
metadata:
{
	minimum-version: [version number]
	command: commandName
	{
		command-option-1: value1
		command-option-2: value2
	}
}
```
```powershell
[install path]\Petrichor> Petrichor.exe input.txt
```

This is equivalent to the following:
```petrichor
metadata:
{
	minimum-version: [version number]
}
```
```powershell
[install path]\Petrichor> Petrichor.exe commandName --inputFile input.txt --commandOption1 value1 --commandOption2 value2
```

---
## Module options region

(OPTIONAL)

The optional `module-options` token allows you to configure module-specific options, if supported by a module.

Each module that supports this region will have its own version of it.

See the relevant module's documentation for more information.

---
## Module-specific tokens

Modules have unique tokens that are specific to their functions.

See the relevant module's documentation for information about its tokens.
