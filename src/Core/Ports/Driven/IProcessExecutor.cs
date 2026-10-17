namespace CodeCrafters.Shell.src.Core.Ports.Driven;

// Driven port: runs an external program, optionally redirecting its output and errors to files.
public interface IProcessExecutor
{
    void Execute(string executablePath, string[] arguments, string? outputFile, string? errorFile, bool appendOutput);
}
