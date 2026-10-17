namespace CodeCrafters.Shell.src.Core.Ports.Driving;

// Driving port: how an adapter asks which command names can complete what the user has typed.
public interface ICommandCompleter
{
    // Returns the built-in and PATH command names that start with the prefix, sorted and without duplicates.
    List<string> FindMatches(string prefix);

    // Returns the longest prefix shared by all the matches.
    string FindLongestCommonPrefix(List<string> matches);
}
