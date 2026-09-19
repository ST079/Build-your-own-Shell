namespace Commands.Echo;

public class EchoCommand
{
    public void run(string command)
    {
        Console.WriteLine($"{command.Substring(5)}");
    }
}