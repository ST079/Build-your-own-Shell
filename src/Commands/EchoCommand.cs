namespace Commands.Echo;

public class EchoCommand
{
    public void Execute(string[] arguments)
    {
        Console.WriteLine(string.Join(" ", arguments));
    }

}