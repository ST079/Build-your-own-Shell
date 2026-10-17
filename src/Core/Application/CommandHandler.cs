using CodeCrafters.Shell.src.Core.Application.BuiltIns;
using CodeCrafters.Shell.src.Core.Domain;
using CodeCrafters.Shell.src.Core.Ports.Driven;
using CodeCrafters.Shell.src.Core.Ports.Driving;

namespace CodeCrafters.Shell.src.Core.Application;

// Runs one line of input: parses it, then hands it to a built-in command or an external program.
public class CommandHandler(
    IOutputWriter output,
    IFileSystem fileSystem,
    IExecutableFinder executableFinder,
    IProcessExecutor processExecutor) : ICommandHandler
{
    private readonly CommandParser commandParser = new();
    private readonly EchoCommand echoCommand = new(output);
    private readonly TypeCommand typeCommand = new(output, executableFinder);
    private readonly PwdCommand pwdCommand = new(output, fileSystem);
    private readonly CdCommand cdCommand = new(output, fileSystem);

    public bool Handle(string input)
    {
        var parsedCommand = commandParser.Parse(input);

        if (parsedCommand.Name == "echo" && parsedCommand.OutputFile is null && parsedCommand.ErrorFile is null)
        {
            echoCommand.Execute(parsedCommand.Arguments);

            return false;
        }

        if (parsedCommand.Name == "exit")
        {
            return true;
        }

        if (parsedCommand.Name == "type")
        {
            typeCommand.Execute(parsedCommand.Arguments);
            return false;
        }

        if (parsedCommand.Name == "pwd")
        {
            pwdCommand.Execute();
            return false;
        }

        if (parsedCommand.Name == "cd")
        {
            cdCommand.Execute(parsedCommand.Arguments);
            return false;
        }

        string? executablePath = executableFinder.Find(parsedCommand.Name);

        if (executablePath is not null)
        {
            processExecutor.Execute(executablePath, parsedCommand.Arguments, parsedCommand.OutputFile, parsedCommand.ErrorFile, parsedCommand.AppendOutput);
            return false;
        }

        output.WriteLine($"{parsedCommand.Name}: command not found");

        return false;
    }
}
