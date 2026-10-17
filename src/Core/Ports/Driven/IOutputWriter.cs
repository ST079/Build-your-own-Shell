namespace CodeCrafters.Shell.src.Core.Ports.Driven;

// Driven port: where the shell writes the output of built-in commands and its error messages.
public interface IOutputWriter
{
    void WriteLine(string text);
}
