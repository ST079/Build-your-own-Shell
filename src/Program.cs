class Program
{
    static void Main()
    {
        // TODO: Uncomment the code below to pass the first stage
        while (true)
        {
            Console.Write("$ ");
            string? command = Console.ReadLine();
            if (command?.Length > 0)
            {
                Console.WriteLine($"{command}: command not found");
            }
        }
    }
}
