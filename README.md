[![progress-banner](https://backend.codecrafters.io/progress/shell/f22641ac-a0d0-4da8-8325-89dd319074d4)](https://app.codecrafters.io/users/ST079?r=2qF)

# Build Your Own Shell in C#

A POSIX-style command-line shell written in C# on .NET 10, built for the
[CodeCrafters "Build Your Own Shell" challenge](https://app.codecrafters.io/courses/shell/overview).

It shows a `$ ` prompt, parses what you type the way a real shell does, runs builtin commands
and external programs, redirects output to files, and completes command names with Tab. The
code follows a hexagonal (ports and adapters) architecture, which keeps the shell's logic
separate from the terminal, the file system and other processes.

## Features

- **Interactive prompt**: a `$ ` prompt with key-by-key line editing (typing, Backspace, Enter).
- **Builtins**: `echo`, `exit`, `type`, `pwd` and `cd` (absolute paths, relative paths and `~`).
- **External programs**: any executable found on `PATH`, run with its arguments.
- **Quoting**: single quotes, double quotes (with `\"` and `\\` escapes) and backslash escapes
  outside quotes.
- **Redirection**: `>`, `1>`, `>>` and `1>>` for stdout, `2>` and `2>>` for stderr (for external
  programs and `echo`).
- **Tab completion** of builtin and `PATH` command names. A single match is completed with a
  trailing space, and several matches are completed up to their longest common prefix. When
  nothing more can be added, the first Tab rings the bell and the second lists every match.

Not implemented yet: pipelines, command history, variable expansion, globbing and job control.

## Example

```console
$ echo 'hello    world' "it's \"quoted\""
hello    world it's "quoted"
$ type echo cat nope
echo is a shell builtin
cat is /bin/cat
nope: not found
$ cat missing.txt 2> errors.txt
$ cat errors.txt
cat: missing.txt: No such file or directory
$ cd ~
$ pwd
/home/you
$ exit
```

## Running it

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
./your_program.sh
```

This builds the project in Release mode into `/tmp/codecrafters-build-csharp` and starts the
shell. Run it in a real terminal: input is read one key at a time (that is how tab completion
works), so piping commands into it is not supported.

To run the CodeCrafters tests for the current stage (requires the CodeCrafters CLI):

```sh
codecrafters submit
```

On the CodeCrafters servers, `.codecrafters/compile.sh` and `.codecrafters/run.sh` perform the
same build and run steps.

## Architecture

The project uses **hexagonal architecture**, also called *ports and adapters*. The shell's
logic lives in a core that does not know it is running in a terminal. The core talks to the
outside world only through small interfaces called **ports**, and **adapters** connect those
ports to real I/O.

- **Core** (`src/Core`): the domain (parsing a line, the list of builtins) and the application
  use cases (running a command, finding completions). It never calls `Console`, `File`,
  `Directory`, `Process` or `Environment` directly.
- **Ports** (`src/Core/Ports`): interfaces owned by the core. *Driving* ports are what the
  outside world can ask the core to do; *driven* ports are what the core needs from the
  outside world.
- **Adapters** (`src/Adapters`): the code that does real I/O. The *driving* adapter is the
  interactive console that calls into the core. The *driven* adapters implement the driven
  ports: they write to stdout, use the file system, search `PATH` and start processes.
- **Composition root** (`src/Program.cs`): the only place that creates the concrete adapters
  and wires them into the core.

```text
                     keyboard
                     │
┌────────────────────▼───────────────────┐
│ ConsoleRepl                            │  driving adapter   Adapters/Driving/
└────────────────────┬───────────────────┘
                     │ calls
┌────────────────────▼───────────────────┐
│ ICommandHandler · ICommandCompleter    │  driving ports     Core/Ports/Driving/
│                                        │
│ CommandHandler · CommandCompleter      │  application       Core/Application/
│ BuiltIns: echo · type · pwd · cd       │
│                                        │
│ CommandParser · ParsedCommand          │  domain            Core/Domain/
│ BuiltInCommands                        │
│                                        │
│ IOutputWriter · IFileSystem            │  driven ports      Core/Ports/Driven/
│ IExecutableFinder · IProcessExecutor   │
└────────────────────▲───────────────────┘
                     │ implements
┌────────────────────┴───────────────────┐
│ ConsoleOutputWriter · LocalFileSystem  │  driven adapters   Adapters/Driven/
│ PathExecutableFinder · ProcessExecutor │
└────────────────────┬───────────────────┘
                     ▼
terminal · file system · PATH · child processes
```

Both arrows that touch the core point into it: adapters depend on the core's interfaces, and
the core never references an adapter. This means the logic can be tested without a terminal,
and an adapter can be replaced (for example, by a fake file system in a test) without changing
the core.

### How a line of input flows

1. `ConsoleRepl` prints `$ ` and reads keys until Enter. On Tab it asks `ICommandCompleter`
   for matching command names and displays the result.
2. It passes the finished line to `ICommandHandler.Handle`.
3. `CommandHandler` uses `CommandParser` to turn the line into a `ParsedCommand`: the command
   name, its arguments and any redirection targets.
4. Builtins run inside the core and write through `IOutputWriter`. Any other name is looked up
   with `IExecutableFinder` and started with `IProcessExecutor`. If nothing is found, the shell
   prints `<name>: command not found`.
5. `Handle` returns `true` for `exit`, which ends the loop.

## Project structure

```text
src/
├── Program.cs                        # Composition root: wires the adapters into the core
├── Core/                             # Inside the hexagon: shell logic, no direct I/O
│   ├── Domain/
│   │   ├── BuiltInCommands.cs        # Names of the builtin commands
│   │   ├── CommandParser.cs          # Tokenizes a line: quotes, escapes, redirections
│   │   └── ParsedCommand.cs          # Command name, arguments and redirection targets
│   ├── Application/
│   │   ├── CommandHandler.cs         # Runs a line as a builtin or an external program
│   │   ├── CommandCompleter.cs       # Finds tab-completion candidates
│   │   └── BuiltIns/                 # EchoCommand, TypeCommand, PwdCommand, CdCommand
│   └── Ports/
│       ├── Driving/                  # What the outside world can ask the core to do
│       │   ├── ICommandHandler.cs
│       │   └── ICommandCompleter.cs
│       └── Driven/                   # What the core needs from the outside world
│           ├── IOutputWriter.cs
│           ├── IFileSystem.cs
│           ├── IExecutableFinder.cs
│           └── IProcessExecutor.cs
└── Adapters/                         # Outside the hexagon: real console, file system, processes
    ├── Driving/
    │   └── ConsoleRepl.cs            # Prompt, line editing and tab-completion display
    └── Driven/
        ├── ConsoleOutputWriter.cs    # IOutputWriter     → stdout
        ├── LocalFileSystem.cs        # IFileSystem       → working directory and $HOME
        ├── PathExecutableFinder.cs   # IExecutableFinder → searches $PATH
        └── ProcessExecutor.cs        # IProcessExecutor  → starts child processes
```
