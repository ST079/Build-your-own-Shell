namespace Execution;

public class ExecutableFinders
{
    public string? Find(string command)
    {
        string? path = Environment.GetEnvironmentVariable("PATH");

        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        string[] directories = path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (string directory in directories)
        {
            string fullPath = Path.GetFullPath(Path.Combine(directory, command));

            if (!File.Exists(fullPath))
            {
                continue;
            }

            if (IsExecutable(fullPath))
            {
                return fullPath;
            }
        }

        return null;
    }

    private bool IsExecutable(string filePath)
    {
        try
        {
            UnixFileMode mode = File.GetUnixFileMode(filePath);

            return (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
        }
        catch (PlatformNotSupportedException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}