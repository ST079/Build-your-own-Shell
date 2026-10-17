namespace CodeCrafters.Shell.src.Core.Ports.Driven;

// Driven port: finds the external programs the shell can run.
public interface IExecutableFinder
{
    // Returns the full path of the command's executable, or null when it cannot be found.
    string? Find(string command);

    // Returns the names of the executables that start with the prefix, or null when PATH is not set.
    List<string>? FindExecutableNames(string prefix);
}
