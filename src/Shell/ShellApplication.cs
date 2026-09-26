using Commands;
using Commands.Echo;
using Commands.Type;
using Execution;

namespace Shell;

public class ShellApplication
{
    private readonly CommandParser commandParser = new();
    private readonly EchoCommand echoCommand = new();
    private readonly TypeCommand typeCommand = new();
    private readonly ExecutableFinders executableFinders = new();
    private readonly ProcessExecutor processExecutor = new();
    private readonly PwdCommand pwdCommand = new();
    private readonly CdCommand cdCommand = new();

    public void Run()
    {
        bool isExit = false;

        while (!isExit)
        {
            Console.Write("$ ");

            //takes the user input.
            string? input = Console.ReadLine();

            //checks if the input has value or not
            // if no value or whitespace skip the latter part.
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            isExit = HandleCommand(input);
        }
    }

    private bool HandleCommand(string input)
    {
        var parsedCommand = commandParser.Parse(input);

        if (parsedCommand.Name == "echo")
        {
            if (parsedCommand.OutputFile is not null)
            {
                string output = string.Join(" ", parsedCommand.Arguments);
                File.WriteAllText(parsedCommand.OutputFile, output + Environment.NewLine);
            }
            else if (parsedCommand.ErrorFile is not null)
            {
                string output = string.Join(" ", parsedCommand.Arguments);
                File.WriteAllText(parsedCommand.ErrorFile, output);
            }
            else
            {
                echoCommand.Execute(parsedCommand.Arguments);
            }
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

        string? executablePath = executableFinders.Find(parsedCommand.Name);

        if (executablePath is not null)
        {
            processExecutor.Execute(executablePath, parsedCommand.Arguments, parsedCommand.OutputFile, parsedCommand.ErrorFile);
            return false;
        }

        Console.WriteLine($"{parsedCommand.Name}: command not found");

        return false;
    }
}