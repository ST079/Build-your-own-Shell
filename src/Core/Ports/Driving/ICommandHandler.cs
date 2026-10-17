namespace CodeCrafters.Shell.src.Core.Ports.Driving;

// Driving port: how an adapter (such as the console prompt) asks the shell to run a line of input.
public interface ICommandHandler
{
    // Parses and runs the input. Returns true when the shell should exit.
    bool Handle(string input);
}
