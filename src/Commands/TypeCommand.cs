namespace Commands.Type;

public class TypeCommand
{
    public void Execute(string[] argument)
    {
        var command = argument[0];

        if (string.Equals(command, "echo") || string.Equals(command, "exit")|| string.Equals(command, "type"))
        {
            Console.WriteLine($"{command} is a shell builtin");
        }
    }
}