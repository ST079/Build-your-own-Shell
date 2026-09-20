namespace Shell;

public class CommandParser
{
    public ParsedCommand Parse(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new ParsedCommand
        {
            Name = parts[0],
            Arguments = parts.Skip(1).ToArray(),
        };
    }
}