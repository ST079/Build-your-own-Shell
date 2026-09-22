namespace Commands;

public class CdCommand
{
    public void Execute(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            return;
        }

        string path = arguments[0];

        if (path == "~")
        {
            path = Environment.GetEnvironmentVariable("HOME") ?? "";
        }

        if (Directory.Exists(path))
        {
            Directory.SetCurrentDirectory(path);
            return;
        }

        Console.WriteLine($"cd: {path}: No such file or directory");
    }
}