namespace Shell;

public class CommandParser
{
    public ParsedCommand Parse(string input)
    {
        // var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        // return new ParsedCommand
        // {
        //     Name = parts[0],
        //     Arguments = parts.Skip(1).ToArray(),
        // };

        var tokens = new List<string>();
        var current = new List<char>();

        bool insideSingleQuotes = false;
        bool insideDoubleQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            var character = input[i];

            if (character == '\\')
            {
                // Inside double quotes:
                // only \" and \\ are special.
                if (insideDoubleQuotes)
                {
                    if (i + 1 < input.Length &&
                        (input[i + 1] == '"' || input[i + 1] == '\\'))
                    {
                        i++;
                        current.Add(input[i]);
                        continue;
                    }

                    // Other backslashes stay literal.
                    current.Add(character);
                    continue;
                }

                // Outside quotes:
                // backslash escapes any next character.
                if (!insideSingleQuotes)
                {
                    i++;

                    if (i < input.Length)
                    {
                        current.Add(input[i]);
                    }

                    continue;
                }
            }
            
            if (Equals(character, '\'') && !insideDoubleQuotes)
            {
                insideSingleQuotes = !insideSingleQuotes;
                continue;
            }

            if (Equals(character, '\"') && !insideSingleQuotes)
            {
                insideDoubleQuotes = !insideDoubleQuotes;
                continue;
            }

            if (char.IsWhiteSpace(character) && !insideSingleQuotes && !insideDoubleQuotes)
            {
                if (current.Count > 0)
                {
                    tokens.Add(new string(current.ToArray()));
                    current.Clear();
                }
                continue;
            }

            current.Add(character);
        }

        if (current.Count > 0)
        {
            tokens.Add(new string(current.ToArray()));
        }

        return new ParsedCommand
        {
            Name = tokens[0],
            Arguments = tokens.Skip(1).ToArray(),
        };


    }
}