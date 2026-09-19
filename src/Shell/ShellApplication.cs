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
            var echo = new EchoCommand();

            if (command?.Length > 0)
            {
                echo.run(command!);
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

        // Console.WriteLine($"{command}: command not found");

        return false;
    }
}