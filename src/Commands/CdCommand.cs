namespace Commands;

public class CdCommand
{
    public void Execute(string[] arguments)
    {
        string path = arguments[0];

        if (Directory.Exists(path))
        {
            Directory.SetCurrentDirectory(path);
            return;
        }

        Console.WriteLine($"cd: {path}: No such file or directory");
    }
}