using CodeCrafters.Shell.src.Core.Ports.Driven;

namespace CodeCrafters.Shell.src.Adapters.Driven;

// Driven adapter: writes the shell's output to the console (stdout).
public class ConsoleOutputWriter : IOutputWriter
{
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }
}
