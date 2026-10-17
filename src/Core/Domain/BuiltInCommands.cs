namespace CodeCrafters.Shell.src.Core.Domain;

public static class BuiltInCommands
{
    public static readonly HashSet<string> Names = [
        "echo",
        "exit",
        "type",
        "pwd",
        "cd",
    ];
}