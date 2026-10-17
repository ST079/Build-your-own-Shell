using CodeCrafters.Shell.src.Core.Domain;
using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Core.Application.BuiltIns;

public class TypeCommand(IOutputWriter output, IExecutableFinder executableFinder)
{
    public void Execute(string[] arguments)
    {
        foreach (string command in arguments)
        {
            if (BuiltInCommands.Names.Contains(command))
            {
                output.WriteLine($"{command} is a shell builtin");
                continue;
            }

            string? executablePath = executableFinder.Find(command);

            if (executablePath is not null)
            {
                output.WriteLine($"{command} is {executablePath}");
            }
            else
            {
                output.WriteLine($"{command}: not found");
            }
        }
    }
}
