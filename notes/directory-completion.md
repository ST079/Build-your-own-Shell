# Directory completion (stage #lc6)

## Goal

When the single match is a directory, complete it with a trailing `/` and **no** space:

```
$ ls <TAB>        →  $ ls pig/
$ ls pig/<TAB>    →  $ ls pig/dog/
$ cat re<TAB>     →  $ cat readme.txt      (files still get a space)
```

## Step-by-step plan

1. **Driven port: ask for directories too.** In `src/Core/Ports/Driven/IFileSystem.cs`, add
   `List<string> GetDirectoryNames(string directory);`
2. **Driven adapter: implement it.** In `src/Adapters/Driven/LocalFileSystem.cs`, copy `GetFileNames`, but loop over `Directory.GetDirectories(directory)`. `Path.GetFileName` gives the last segment (`dog`), just as it does for files.
3. **Core: add the missing-directory guard now.** This was step 3 of the last stage, and it's still missing. `ls nope/<TAB>` makes `Directory.GetFiles` throw and kills the shell. Add `if (!fileSystem.DirectoryExists(directory)) return [];`
4. **Core: build one list of entries.** In `FindFileMatches`, combine:
   - the file names, unchanged (`readme.txt`)
   - the directory names with `/` appended (`pig/`)

   Then run the same `Where` (on `namePrefix`), `Select` (put `directoryPart` back) and `OrderBy` as before.
5. **Driving adapter: decide on the trailing space.** In `ConsoleRepl.AutoComplete`, for the single-match case, use
   `addSpace: !matches[0].EndsWith('/')`.
   A directory already ends in `/`, which `AppendCompletion` types, so it only needs to skip the space.
6. **Update comments.** Update the comment in `ICommandCompleter.FindFileMatches` to say "files and directories, directories end in /".
7. **Test in a real terminal** with the table below, then run `codecrafters submit`.

## Key logic

```csharp
// CommandCompleter.FindFileMatches
if (!fileSystem.DirectoryExists(directory)) return [];

var entries = fileSystem.GetFileNames(directory)
    .Concat(fileSystem.GetDirectoryNames(directory).Select(name => name + "/"));

return entries
    .Where(name => name.StartsWith(namePrefix, StringComparison.Ordinal))
    .Select(name => directoryPart + name)
    .OrderBy(name => name, StringComparer.Ordinal)
    .ToList();
```

```csharp
// ConsoleRepl, one match
AppendCompletion(input, word, matches[0], addSpace: !matches[0].EndsWith('/'));
```

## Why

- **The `/` is part of the match.** Storing `pig/` and not `pig` means `AppendCompletion` types the slash for free. The next Tab sees `pig/`, splits at the last `/`, and searches inside `pig`. The previous stage's logic handles the nested level with no new code.
- **No space after a directory.** The user usually wants to keep going deeper. A space would end the word and the next Tab would start a fresh, empty word.
- **The port returns names, the core adds `/`.** `LocalFileSystem` only reports facts ("these are directories"). How a directory is *displayed in completion* is a shell rule, so it belongs in the core.
- **The adapter decides spacing from the string.** It doesn't need a new "isDirectory" flag. A trailing `/` already tells it what kind of entry it is.
- **Word `""` works.** `ls <TAB>`: the word after the space is empty, so `namePrefix` is `""` and everything in the cwd matches. In the test that's only `pig/`.

## Where it lives in the hexagon

| Piece | Layer | Change |
| --- | --- | --- |
| `IFileSystem.GetDirectoryNames` | Core / Ports / Driven | new method |
| `LocalFileSystem.GetDirectoryNames` | Adapters / Driven | `Directory.GetDirectories` |
| `CommandCompleter.FindFileMatches` | Core / Application | guard, merge, append `/` |
| `ConsoleRepl.AutoComplete` | Adapters / Driving | `addSpace` depends on `/` |

## Gotchas

- Without the `DirectoryExists` guard, a typo like `ls nope/<TAB>` crashes the shell.
- Your `Path.Combine(directoryPart, name)` is fine (`Path.Combine("", "pig/")` is `"pig/"`). Plain `directoryPart + name` does the same thing.
- Multiple matches (`p<TAB>` with both `pig/` and `pen.txt`) still go through the longest common prefix. That's handled in a later stage.

## Try it

```sh
mkdir -p pig/dog && touch readme.txt
```

| Type               | Expect                 |
| ------------------ | ---------------------- |
| `ls pi<TAB>`       | `ls pig/` (no space)   |
| `ls pig/<TAB>`     | `ls pig/dog/`          |
| `cat re<TAB>`      | `cat readme.txt `      |
| `ls nope/<TAB>`    | bell, no crash         |
