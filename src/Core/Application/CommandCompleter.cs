using CodeCrafters.Shell.src.Core.Domain;
using CodeCrafters.Shell.src.Core.Ports.Driven;
using CodeCrafters.Shell.src.Core.Ports.Driving;

namespace CodeCrafters.Shell.src.Core.Application;

// Finds the command names that can complete what the user has typed so far.
public class CommandCompleter(IExecutableFinder executableFinder, IFileSystem fileSystem) : ICommandCompleter
{
    public List<string> FindMatches(string prefix)
    {
        // Check built-in commands first.
        var matches = BuiltInCommands.Names.Where(command => command.StartsWith(prefix, StringComparison.Ordinal)).ToList();

        // Add external executables found in PATH.
        matches.AddRange(executableFinder.FindExecutableNames(prefix));

        // Remove duplicate and order them.
        return matches
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();
    }

    // files and directories, directories end in /
    public List<string> FindFileMatches(string prefix)
    {
        // // Look at the files in the directory the shell is currently in.
        // List<string> fileNames = fileSystem.GetFileNames(fileSystem.GetCurrentDirectory());

        int lastSlash = prefix.LastIndexOf('/');
        string directoryPart = lastSlash >= 0 ? prefix[..(lastSlash + 1)] : string.Empty;
        string namePrefix = lastSlash >= 0 ? prefix[(lastSlash + 1)..] : prefix;

        string directory = Path.Combine(fileSystem.GetCurrentDirectory(), directoryPart);

        if (!fileSystem.DirectoryExists(directory))
        {
            return [];
        }

        // Directories end in "/" so the completion types the slash and the next tab can ge deeper
        var entries = fileSystem.GetFileNames(directory)
                                .Concat(fileSystem.GetDirectoryNames(directory).Select(name => name + "/"));

        // keep the ones that start with what the user typed, sorted like the command matches.
        return entries
        .Where(name => name.StartsWith(namePrefix, StringComparison.Ordinal))
        .Select(name => Path.Combine(directoryPart, name)) // give back the full partial path.
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToList();
    }

    public string FindLongestCommonPrefix(List<string> matches)
    {
        if (matches.Count == 0)
        {
            return string.Empty;
        }

        string prefix = matches[0];

        for (int i = 1; i < matches.Count; i++)
        {
            int commonLength = 0;
            int maxLength = Math.Min(prefix.Length, matches[i].Length);
            while (commonLength < maxLength && prefix[commonLength] == matches[i][commonLength])
            {
                commonLength++;
            }

            prefix = prefix[..commonLength];

            if (prefix.Length == 0)
            {
                break;
            }
        }
        return prefix;
    }

}
