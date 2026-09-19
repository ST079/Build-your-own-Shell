using Commands.Echo;

namespace Shell;

public class ShellApplication
{
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
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string command = parts[0];

        string[] arguments = parts.Skip(1).ToArray();

        if (command.StartsWith("echo "))
        {
            var echo = new EchoCommand();
            echo.Execute(arguments);
            return false;
        }

        if (command == "exit")
        {
            return true;
        }
        
        Console.WriteLine($"{command}: command not found");

        return false;
    }
}