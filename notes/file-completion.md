# File completion

**Stage goal:** pressing Tab while typing an *argument* completes a filename from the current
directory and adds a trailing space.

```console
$ cat re<TAB>
$ cat readme.txt 
$ xyz read<TAB>          # works after any command, even one that doesn't exist
$ xyz readme.txt 
```

Scope for this stage: files in the current directory only, and only the single-match case.
Directories, nested paths and multiple matches come in later stages.

## Key logic

**1. Find the word being completed: the text after the last space.**

```csharp
int lastSpace = line.LastIndexOf(' ');
bool isArgument = lastSpace >= 0;
string word = isArgument ? line[(lastSpace + 1)..] : line;
```

`"cat re"` gives `isArgument = true` and `word = "re"`. `"ech"` has no space, so it is still the
command word and goes through command completion as before.

**2. Pick where the candidates come from.**

| Word position | Candidates |
| --- | --- |
| First word (no space yet) | builtin names and executables on `PATH` (already implemented) |
| Any later word | names of the files in the current directory (new) |

**3. Keep the candidates that start with the word.**

```csharp
fileNames.Where(name => name.StartsWith(word, StringComparison.Ordinal))
```

**4. One match: print only the missing characters, then a space.**

For `word = "re"` and the match `readme.txt`, append `"adme.txt"` (that is, `match[word.Length..]`)
and then `' '`. `AppendCompletion` in `ConsoleRepl` already does exactly this, as long as you pass
it the *word*, not the whole line.

## Why we do it this way

- **Why the text after the last space?** A shell completes the word under the cursor. Our line
  editor has no arrow keys, so the cursor is always at the end and the last word is the one being
  typed. Everything before it is finished and must not change.
- **Why do commands and files have separate sources?** In a shell, the first word is a program
  name, looked up among the builtins and on `PATH`. Later words are arguments, and most often file
  paths. Bash makes the same choice by default.
- **Why append only the missing characters?** The terminal already shows what the user typed.
  Printing just the rest keeps the screen and the `input` buffer in sync. Reprinting the whole word
  would show it twice.
- **Why the trailing space?** It tells the user the word is complete, and they can type the next
  argument straight away. That's also how bash behaves.
- **Why `StringComparison.Ordinal`?** File names on Linux, where CodeCrafters runs the tests, are
  compared character by character and are case-sensitive. Ordinal comparison does exactly that.
  The default culture-aware comparison can treat some characters as equal when the file system
  does not.
- **Why `Directory.GetFiles` and not `GetFileSystemEntries`?** This stage asks for files only.
  Directories need extra handling (a trailing `/` instead of a space), which comes in a later
  stage.

## Where each piece lives in the hexagon

| Piece | Layer | Why it belongs there |
| --- | --- | --- |
| `IFileSystem.GetFileNames(directory)` | Driven port | The core needs a folder listing but must not call `Directory` itself |
| `LocalFileSystem.GetFileNames` | Driven adapter | Does the real disk I/O |
| `CommandCompleter.FindFileMatches(prefix)` | Application (core) | The matching rule is shell logic, testable without a disk |
| `ICommandCompleter.FindFileMatches(prefix)` | Driving port | Lets the console ask the core for file matches |
| `ConsoleRepl.AutoComplete` | Driving adapter | Finds the word under the cursor and displays the result, the same split as bash's readline |
| `Program.cs` | Composition root | Passes the file system adapter into `CommandCompleter` |

The adapter only *lists* names; the core decides which names match. That keeps the adapter free of
logic, so a test can use a fake file system that returns a fixed list.

## Gotchas

- `Directory.GetFiles` returns **full paths**. Use `Path.GetFileName` to get just `readme.txt`.
- An empty word (`cat <TAB>`) matches every file. One file completes; several ring the bell.
- After `cd`, completion lists the new directory, because `Directory.GetCurrentDirectory()`
  reflects `Directory.SetCurrentDirectory()`.
- The shell's current directory is wherever you launched it from. `your_program.sh` builds inside
  the repo but runs the shell in your current directory.

## Try it

From the repo root, run `./your_program.sh`, then:

| Type | Expect |
| --- | --- |
| `cat REA<TAB>` | `cat README.md ` |
| `xyz your<TAB>` | `xyz your_program.sh ` |
| `ech<TAB>` | `echo ` (command completion still works) |
| `cat zzz<TAB>` | bell, input unchanged |

Then run `codecrafters submit`.

## Coming in later stages

- **Multiple matches:** longest common prefix, then the list on a second Tab. The command path
  already does this, so if files go through the same flow, it works for them too.
- **Directories and nested paths:** `cd sr<TAB>` → `cd src/`, and `cat src/Co<TAB>`. You will then
  need to split the word on the last `/` and list *that* directory, which is why
  `GetFileNames` takes a directory argument.
