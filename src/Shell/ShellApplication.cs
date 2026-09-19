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

            string? command = Console.ReadLine();

            if (command?.Length > 0)
            {
                isExit = HandleCommand(command);
            }
        }
    }

    private bool HandleCommand(string command)
    {
        if (string.Equals(command, "exit"))
        {
            return true;
        }

        if (command.StartsWith("echo "))
        {
            var echo = new EchoCommand();
            echo.run(command!);
        }
        else
        {
            

        Console.WriteLine($"{command}: command not found");
        }

        return false;
    }
}