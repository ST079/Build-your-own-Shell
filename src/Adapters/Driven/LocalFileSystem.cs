using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Adapters.Driven;

// Driven adapter: the real file system and environment of the machine the shell runs on.
public class LocalFileSystem : IFileSystem
{
    public string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    public void SetCurrentDirectory(string path)
    {
        Directory.SetCurrentDirectory(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public string? GetHomeDirectory()
    {
        return Environment.GetEnvironmentVariable("HOME");
    }

    public List<string> GetFileNames(string directory)
    {
        var fileNames = new List<string>();

        // GetFiles returns full path, so keep only the name part.
        foreach (string filePath in Directory.GetFiles(directory))
        {
            fileNames.Add(Path.GetFileName(filePath));
        }

        return fileNames;
    }
}
