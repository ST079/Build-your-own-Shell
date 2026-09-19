class Program
{
    static void Main()
    {
        bool isExit = false;
        while (!isExit)
        {
            Console.Write("$ ");
            string? command = Console.ReadLine();
            if (command?.Length > 0)
            {
                if (string.Equals(command, "exit"))
                {
                    isExit = true;
                }
                else
                {
                    Console.WriteLine($"{command}: command not found");
                }
            }
        }
    }
}
