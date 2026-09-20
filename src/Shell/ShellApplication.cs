using Commands.Echo;
using Commands.Type;

namespace Shell;

public class ShellApplication
{
    private readonly CommandParser commandParser = new();
    private readonly EchoCommand echoCommand = new();
    private readonly TypeCommand typeCommand = new();

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

        Console.WriteLine($"{parsedCommand.Name}: command not found");

        return false;
    }
}