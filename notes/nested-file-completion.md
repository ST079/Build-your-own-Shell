# Nested file completion (stage #ue6)

## Goal

Complete file names inside subdirectories when the word being completed contains a `/`:

```
$ cat path/to/f<TAB>
$ cat path/to/file.txt 
```

## Step-by-step plan

Only `FindFileMatches` in `src/Core/Application/CommandCompleter.cs` changes. `ConsoleRepl` already passes in the whole last word (`path/to/f`).

1. **Split the word at the last `/`.** Use `LastIndexOf('/')` to get `directoryPart` (up to and including the `/`) and `namePrefix` (the rest). With no `/`, `directoryPart` is `""` and `namePrefix` is the whole word, so the previous stage keeps working.
2. **Work out the directory to search:** `Path.Combine(fileSystem.GetCurrentDirectory(), directoryPart)`.
3. **Guard against a missing directory.** If `!fileSystem.DirectoryExists(directory)`, return an empty list. The shell then rings the bell instead of crashing.
4. **Filter on `namePrefix`.** Change the `.Where(...)` to compare against `namePrefix`, not `prefix`.
5. **Put the directory back on each match:** `.Select(name => directoryPart + name)`. The result is `path/to/file.txt`, which is what `AppendCompletion` expects.
6. **Update the comment** on `FindFileMatches` in `ICommandCompleter.cs`. It should say the prefix may include a directory path.
7. **Test in a real terminal** (`./your_program.sh`) using the "Try it" table below.
8. **Submit:** `codecrafters submit`.

## Key logic

Split the word at the **last** `/`:

| Word          | Directory part | Name prefix |
| ------------- | -------------- | ----------- |
| `path/to/f`   | `path/to/`     | `f`         |
| `re`          | `""`           | `re`        |
| `/tmp/a`      | `/tmp/`        | `a`         |

```csharp
int lastSlash = prefix.LastIndexOf('/');
string directoryPart = lastSlash >= 0 ? prefix[..(lastSlash + 1)] : "";
string namePrefix    = lastSlash >= 0 ? prefix[(lastSlash + 1)..] : prefix;

string directory = Path.Combine(fileSystem.GetCurrentDirectory(), directoryPart);
if (!fileSystem.DirectoryExists(directory)) return [];

return fileSystem.GetFileNames(directory)
    .Where(name => name.StartsWith(namePrefix, StringComparison.Ordinal))
    .Select(name => directoryPart + name)      // give back the full partial path
    .OrderBy(name => name, StringComparer.Ordinal)
    .ToList();
```

## Why

- **Last `/`, not first.** Only the final segment is still being typed. Everything before it is a finished directory path.
- **Keep the `/` in the directory part.** Then `directoryPart + name` rebuilds the path without adding a separator yourself.
- **Return `directoryPart + name`.** `AppendCompletion` types `completion[word.Length..]`, the characters after what the user typed. That only works if the match starts with exactly what the user typed. This is why `ConsoleRepl` didn't need to change.
- **`Path.Combine` with the cwd.** The stage says the path is relative to the working directory. As a bonus, if the second argument is absolute (`/tmp/`), `Path.Combine` returns it unchanged, so absolute paths work for free.
- **`DirectoryExists` guard.** `Directory.GetFiles` throws on a missing directory, and an exception would crash the shell on a typo.
- **The longest common prefix still works.** All matches share the same `directoryPart`, so the common prefix always includes it.

## Where it lives in the hexagon

| Piece | Layer | Change |
| --- | --- | --- |
| `CommandCompleter.FindFileMatches` | Core / Application | split, guard, re-prefix |
| `IFileSystem.DirectoryExists` | Core / Ports / Driven | already existed, reused |
| `ConsoleRepl` | Adapters / Driving | **no change** |

The rule "what does a path mean" is shell logic, so it belongs in the core. The adapter only finds the last word and types characters.

## Gotchas

- Run in a real terminal (`./your_program.sh`), not the VS Code internal console, because `Console.ReadKey` needs a TTY.
- `GetFileNames` returns files only. `path/<TAB>` won't complete to the `to/` directory yet.

## Try it

```sh
mkdir -p path/to && touch path/to/file.txt
```

| Type                  | Expect                     |
| --------------------- | -------------------------- |
| `cat path/to/f<TAB>`  | `cat path/to/file.txt `    |
| `cat re<TAB>`         | still completes as before  |
| `cat nope/x<TAB>`     | bell, no crash             |

## Coming next

A later stage will probably complete directory names too, with a trailing `/` instead of a space, and handle multiple matches in nested paths.
