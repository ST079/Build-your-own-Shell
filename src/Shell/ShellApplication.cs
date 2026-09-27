using Commands;
using Commands.Echo;
using Commands.Type;
using Constants.BuiltInCommands;
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

    private int tabPressCount;

    public void Run()
    {
        bool isExit = false;

        while (!isExit)
        {
            Console.Write("$ ");

            //takes the user input.
            // string? input = Console.ReadLine();

            string? input = ReadCommand();

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

        string? executablePath = executableFinders.Find(parsedCommand.Name);

        if (executablePath is not null)
        {
            processExecutor.Execute(executablePath, parsedCommand.Arguments, parsedCommand.OutputFile, parsedCommand.ErrorFile, parsedCommand.AppendOutput);
            return false;
        }

        Console.WriteLine($"{parsedCommand.Name}: command not found");

        return false;
    }

    private string ReadCommand()
    {
        var input = new List<char>();

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                tabPressCount = 0;
                return new String(input.ToArray());
            }

            if (key.Key == ConsoleKey.Tab)
            {
                tabPressCount++;
                AutoComplete(input);
                continue;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                tabPressCount = 0;

                if (input.Count > 0)
                {
                    input.RemoveAt(input.Count - 1);
                    Console.Write("\b \b");
                }
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                tabPressCount = 0;
                input.Add(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    private void AutoComplete(List<char> input)
    {
        string current = new(input.ToArray());

        // Check built-in commands first.
        List<string> matches = BuiltInCommands.Names.Where(command => command.StartsWith(current)).ToList();

        // Add external executables found in PATH.
        matches.AddRange(executableFinders.FindExecutableNames(current)!);

        matches = matches
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        if (matches.Count == 0)
        {
            Console.Write('\x07');
            return;
        }

        // Exactly one match: complete it immediately.
        if (matches.Count == 1)
        {
            string match = matches[0];

            for (int i = current.Length; i < match.Length; i++)
            {
                input.Add(match[i]);
                Console.Write(match[i]);
            }

            input.Add(' ');
            Console.Write(' ');

            tabPressCount = 0;
            return;
        }

        // Multiple matches: first TAB only rings the bell.
        if (tabPressCount == 1)
        {
            Console.Write('\x07');
            return;
        }

        // Second TAB displays all matching commands.
        Console.WriteLine();

        Console.WriteLine(string.Join("  ", matches));

        Console.Write("$ ");
        Console.Write(current);

        tabPressCount = 0;
    }
}