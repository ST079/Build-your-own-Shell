

using Constants.BuiltInCommands;
using Execution;

namespace Commands.Type;

public class TypeCommand
{
    private readonly ExecutableFinders executableFinders = new();
    public void Execute(string[] arguments)
    {
        foreach (string command in arguments)
        {
            if (BuiltInCommands.Names.Contains(command))
            {
                Console.WriteLine($"{command} is a shell builtin");
                continue;
            }

            string? executablePath = executableFinders?.Find(command);

            if (executablePath is not null)
            {
                Console.WriteLine($"{command} is {executablePath}");
            }
            else
            {
                Console.WriteLine($"{command}: not found");
            }
        }
    }
}