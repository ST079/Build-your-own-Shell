

using Constants.BuiltInCommands;

namespace Commands.Type;

public class TypeCommand
{
    public void Execute(string[] arguments)
    {
        foreach (string command in arguments)
        {
            if (BuiltInCommands.Names.Contains(command))
            {
                Console.WriteLine($"{command} is a shell builtin");
            }
            else
            {
                Console.WriteLine($"{command}: not found");
            }
        }
    }
}