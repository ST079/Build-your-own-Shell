using CodeCrafters.Shell.src.Adapters.Driven;
using CodeCrafters.Shell.src.Adapters.Driving;
using CodeCrafters.Shell.src.Core.Application;

namespace CodeCrafters.Shell.src;

// Composition root: creates the adapters and plugs them into the core through its ports.
class Program
{
    static void Main()
    {
        // Driven adapters: how the core reaches the console, the file system and other programs.
        var output = new ConsoleOutputWriter();
        var fileSystem = new LocalFileSystem();
        var executableFinder = new PathExecutableFinder();
        var processExecutor = new ProcessExecutor();

        // Core use cases, given the driven adapters through their ports.
        var commandHandler = new CommandHandler(output, fileSystem, executableFinder, processExecutor);
        var commandCompleter = new CommandCompleter(executableFinder);

        // Driving adapter: the interactive prompt that drives the core.
        var shell = new ConsoleRepl(commandHandler, commandCompleter);
        shell.Run();
    }
}
