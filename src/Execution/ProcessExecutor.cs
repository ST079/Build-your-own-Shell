using System.Diagnostics;

namespace Execution;

public class ProcessExecutor
{
    public void Execute(string executablePath, string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.GetFileName(executablePath),
            UseShellExecute = false,
        };

        foreach ( string argument in arguments)
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