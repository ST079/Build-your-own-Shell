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

    public List<string> FindFileMatches(string prefix)
    {
        // Look at the files in the directory the shell is currently in.
        List<string> fileNames = fileSystem.GetFileNames(fileSystem.GetCurrentDirectory());

        // keep the ones that start with what the user typed, sorted like the command matches.
        return fileNames
        .Where(name => name.StartsWith(prefix, StringComparison.Ordinal))
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
