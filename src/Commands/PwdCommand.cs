namespace Commands;

public class PwdCommand
{
    public void Execute()
    {
        // Shows the current working directory.
        Console.WriteLine(Directory.GetCurrentDirectory());
    }
}