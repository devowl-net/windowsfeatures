using System;

using WindowsFeatures.Implementations;

namespace WindowsFeatures
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            char input;
            PrintHelp();
            
            while ((input = Console.ReadKey().KeyChar) != 'x')
            {
                switch (input)
                {
                    case '1':
                        Console.WriteLine();
                        new DismWrapperExample().Run();
                        break;
                    case '2':
                        Console.WriteLine();
                        new Dism32Under64CmdExample().Run();
                        break;
                }
                PrintHelp();
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Choose an example:");
            Console.WriteLine("1. Microsoft Dism Wrapper.");
            Console.WriteLine("2. Process x32 Windows Features under Windows x64. (P.S. The problem is windows hasn't x32 DISM assembly on Windows x64. So, you need some out of process x64 application. Here is some easy workaround)");
            Console.WriteLine("E[x]it");
        }
    }
}