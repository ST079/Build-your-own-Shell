using CodeCrafters.Shell.src.Core.Ports.Driving;

namespace CodeCrafters.Shell.src.Adapters.Driving;

// Driving adapter: the interactive prompt. It reads key presses from the console, handles
// line editing and tab completion, and hands each finished line to the core.
public class ConsoleRepl(ICommandHandler commandHandler, ICommandCompleter commandCompleter)
{
    private int tabPressCount;

    public void Run()
    {
        bool isExit = false;

        while (!isExit)
        {
            Console.Write("$ ");

            //takes the user input.
            // string? input = Console.ReadLine();

            string? input = ReadCommand();

            //checks if the input has value or not
            // if no value or whitespace skip the latter part.
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            isExit = commandHandler.Handle(input);
        }
    }

    private string ReadCommand()
    {
        var input = new List<char>();

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                tabPressCount = 0;
                return new String(input.ToArray());
            }

            if (key.Key == ConsoleKey.Tab)
            {
                tabPressCount++;
                AutoComplete(input);
                continue;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                tabPressCount = 0;

                if (input.Count > 0)
                {
                    input.RemoveAt(input.Count - 1);
                    Console.Write("\b \b");
                }
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                tabPressCount = 0;
                input.Add(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    private void AutoComplete(List<char> input)
    {
        string current = new(input.ToArray());

        // The word being completed is the text after the last space
        int lastSpace = current.LastIndexOf(' ');
        bool isArgument = lastSpace >= 0;
        string word = isArgument ? current[(lastSpace + 1)..] : current;

        // // Ask the core for the built-in and PATH commands that match the current input.
        // var matches = commandCompleter.FindMatches(current);

        // The first word is a command any later word is a file name.
        var matches = isArgument ? commandCompleter.FindFileMatches(word) : commandCompleter.FindMatches(word);

        // No matches: ring the bell and preserve the current input.
        if (matches.Count == 0)
        {
            Console.Write('\x07');
            return;
        }

        // Exactly one match: complete it immediately.
        if (matches.Count == 1)
        {
            AppendCompletion(input, word, matches[0], addSpace: true);
            tabPressCount = 0;
            return;
        }

        //multiple matches: complete their longest common prefix
        string commonPrefix = commandCompleter.FindLongestCommonPrefix(matches);

        if (commonPrefix.Length > word.Length)
        {
            AppendCompletion(input, word, commonPrefix, addSpace: false);
            tabPressCount = 0;
            return;
        }


        // no additional character can be completed
        // a repeated tab displays all the matching commands.
        if (tabPressCount >= 2)
        {
            Console.WriteLine();

            Console.WriteLine(string.Join("  ", matches));

            Console.Write("$ ");
            Console.Write(new string(input.ToArray()));

            tabPressCount = 0;
        }
        else
        {
            Console.Write('\x07');
        }
    }

    private void AppendCompletion(List<char> input, string word, string completion, bool addSpace)
    {
        // add and display only the characters missing from the word input.
        for (int i = word.Length; i < completion.Length; i++)
        {
            input.Add(completion[i]);
            Console.Write(completion[i]);
        }

        // add a trailing space only when a single match remains.
        if (addSpace)
        {
            input.Add(' ');
            Console.Write(' ');
        }
    }
}