namespace CodeCrafters.Shell.src.Core.Ports.Driven;

// Driven port: the working directory and folders that cd and pwd operate on.
public interface IFileSystem
{
    string GetCurrentDirectory();

    void SetCurrentDirectory(string path);

    bool DirectoryExists(string path);

    // Returns the user's home directory, or null when it is not set.
    string? GetHomeDirectory();

    // Returns the names (not full paths of the files in the directory)
    List<string> GetFileNames(string directory);

    // Returns the names (not full paths) of the directories in the directory.
    List<string> GetDirectoryNames(string directory);
}
