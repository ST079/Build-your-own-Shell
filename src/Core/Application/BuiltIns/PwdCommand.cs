using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Core.Application.BuiltIns;

public class PwdCommand(IOutputWriter output, IFileSystem fileSystem)
{
    public void Execute()
    {
        // Shows the current working directory.
        output.WriteLine(fileSystem.GetCurrentDirectory());
    }
}
