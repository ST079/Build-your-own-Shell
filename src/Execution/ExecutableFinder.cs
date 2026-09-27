namespace Execution;

public class ExecutableFinders
{
    public string? Find(string command)
    {
        // Read the directories where executable commands can be found.
        string? path = Environment.GetEnvironmentVariable("PATH");

        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        // PATH contains multiple directories separated by the platform-specific separator.
        string[] directories = path.Split( Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (string directory in directories)
        {
            string fullPath = Path.GetFullPath(
                Path.Combine(directory, command));

            // The command may not exist in this PATH directory, so continue searching.
            if (!File.Exists(fullPath))
            {
                continue;
            }

            // Only return the file if it has executable permissions.
            if (IsExecutable(fullPath))
            {
                return fullPath;
            }
        }

        return null;
    }

    private bool IsExecutable(string filePath)
    {
        // Unix systems use file permission bits to determine whether a file is executable.
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            try
            {
                UnixFileMode mode = File.GetUnixFileMode(filePath);

                // The file is executable if any user, group, or other execute bit is set.
                return (mode & (
                    UnixFileMode.UserExecute |
                    UnixFileMode.GroupExecute |
                    UnixFileMode.OtherExecute)) != 0;
            }
            catch (PlatformNotSupportedException)
            {
                // The current platform does not support reading Unix file permissions.
                return false;
            }
            catch (IOException)
            {
                // The file permissions could not be read.
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                // The process does not have permission to inspect the file.
                return false;
            }
        }

        // Windows uses a different mechanism for determining executable files.
        return true;
    }

    public string? FindExecutableName(string prefix)
    {
        // Read PATH so we can search for executable names matching the user's input.
        string? path = Environment.GetEnvironmentVariable("PATH");

        if (path is null)
        {
            return null;
        }

        // Execute permissions that indicate a file can be run on Unix systems.
        UnixFileMode executeBits =
            UnixFileMode.UserExecute |
            UnixFileMode.GroupExecute |
            UnixFileMode.OtherExecute;

        // Search every directory listed in PATH.
        foreach (string directory in path.Split(Path.PathSeparator))
        {
            if (!Directory.Exists(directory))
            {
                continue;
            }

            // Inspect the files in this directory for a matching executable.
            foreach (string filePath in Directory.GetFiles(directory))
            {
                string fileName = Path.GetFileName(filePath);

                // Ignore files whose names do not start with the user's input.
                if (!fileName.StartsWith(prefix))
                {
                    continue;
                }

                // Windows does not use Unix execute permission bits.
                if (OperatingSystem.IsWindows())
                {
                    return fileName;
                }

                // On Unix, only complete the name if the file is executable.
                if ((File.GetUnixFileMode(filePath) & executeBits) != 0)
                {
                    return fileName;
                }
            }
        }

        return null;
    }
}