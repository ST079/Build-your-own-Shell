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
        string? outputFile = null;
        string? errorFile = null;

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

            if (Equals(character, '>') && !insideDoubleQuotes && !insideSingleQuotes)
            {
                if (current.Count > 0)
                {
                    string currentToken = new(current.ToArray());

                    if (Equals(currentToken, "1"))
                    {
                        tokens.Add("1>");
                    }
                    else if (Equals(currentToken, "2"))
                    {
                        tokens.Add("2>");
                    }
                    else
                    {
                        tokens.Add(currentToken);
                        tokens.Add(">");
                    }
                    current.Clear();
                }
                else
                {
                    tokens.Add(">");
                }
                continue;
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

        for (int i = 0; i < tokens.Count; i++)
        {
            if (Equals(tokens[i], ">") || Equals(tokens[i], "1>"))
            {
                outputFile = tokens[i + 1];

                tokens.RemoveAt(i + 1);
                tokens.RemoveAt(i);
                i--;
            }
            else if (Equals(tokens[i], "2>"))
            {
                errorFile = tokens[i + 1];

                tokens.RemoveAt(i + 1);
                tokens.RemoveAt(i);
                i--;
            }
        }

        return new ParsedCommand
        {
            Name = tokens[0],
            Arguments = tokens.Skip(1).ToArray(),
            OutputFile = outputFile,
            ErrorFile = errorFile,
        };


    }
}