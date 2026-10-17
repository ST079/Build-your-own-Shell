namespace CodeCrafters.Shell.src.Core.Domain;

public class ParsedCommand
{
    public string Name { get; init; } = string.Empty;
    public string[] Arguments { get; init; } = [];
    public string? OutputFile { get; set; }
    public string? ErrorFile { get; set; }
    public bool AppendOutput { get; set; }
}