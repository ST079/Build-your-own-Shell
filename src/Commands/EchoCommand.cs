namespace Commands.Echo;

public class EchoCommand
{
    public void run(string command)
    {
        if (command.StartsWith("echo "))
        {
            Console.WriteLine($"{command.Substring(5)}");
        }
    }
}