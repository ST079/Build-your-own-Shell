using System.Diagnostics;

namespace Execution;

public class ProcessExecutor
{
    public void Execute(string executablePath, string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
        };

        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add("exec -a \"$0\" \"$@\"");

        // Set argv[0] to only the executable name
        startInfo.ArgumentList.Add(Path.GetFileName(executablePath));

        startInfo.ArgumentList.Add(executablePath);

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo,
        };

        process.Start();
        process.WaitForExit();
    }
}