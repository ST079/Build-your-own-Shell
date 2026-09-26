using System.Diagnostics;

namespace Execution;

public class ProcessExecutor
{
    public void Execute(
        string executablePath,
        string[] arguments,
        string? outputFile)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.GetFileName(executablePath),
            UseShellExecute = false,
            RedirectStandardOutput = outputFile is not null,
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo,
        };

        process.Start();

        if (outputFile is not null)
        {
            string output = process.StandardOutput.ReadToEnd();

            File.WriteAllText(outputFile, output);
        }

        process.WaitForExit();
    }
}