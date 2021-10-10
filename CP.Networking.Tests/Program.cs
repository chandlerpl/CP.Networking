using CP.Common.Commands;
using System;

namespace CP.Networking.Tests
{
    class Program
    {
        static CommandSystem CommandSystem;

        static void Main(string[] args)
        {
            CommandSystem = new CommandSystem();

            while (true)
            {
                Console.Write("Please enter a command: ");
                CommandSystem.CommandInterface(null, Console.ReadLine().Split(' '));
            }
        }
    }
}
