using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Core.Application.BuiltIns;

public class EchoCommand(IOutputWriter output)
{
    public void Execute(string[] arguments)
    {
        output.WriteLine(string.Join(" ", arguments));
    }

}
