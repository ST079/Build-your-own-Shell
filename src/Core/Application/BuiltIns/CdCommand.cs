using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Core.Application.BuiltIns;

public class CdCommand(IOutputWriter output, IFileSystem fileSystem)
{
    public void Execute(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            return;
        }

        string path = arguments[0];

        if (path == "~")
        {
            path = fileSystem.GetHomeDirectory() ?? "";
        }

        if (fileSystem.DirectoryExists(path))
        {
            fileSystem.SetCurrentDirectory(path);
            return;
        }

        output.WriteLine($"cd: {path}: No such file or directory");
    }
}
