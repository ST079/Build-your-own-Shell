using System.Diagnostics;

namespace Execution;

public class ProcessExecutor
{
    public void Execute(string executablePath, string[] arguments, string? outputFile, string? errorFile, bool appendOutput)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.GetFileName(executablePath),
            UseShellExecute = false,
            RedirectStandardOutput = outputFile is not null,
            RedirectStandardError = errorFile is not null,
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

            if (appendOutput)
            {
                File.AppendAllText(outputFile, output);
            }
            else
            {
                File.WriteAllText(outputFile, output);
            }
        }

        if (errorFile is not null)
        {
            string error = process.StandardError.ReadToEnd();

            File.WriteAllText(errorFile, error);
        }

        process.WaitForExit();
    }
}