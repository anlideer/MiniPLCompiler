using System;
using System.IO;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ProgramEntry.ExeProgram(args[0]);
            }
            else
            {
                Console.WriteLine("No command line args provided. Running sample program...");
                ProgramEntry.ExeProgram(System.IO.Path.Combine("..", "..", "..", "TestData", "Example1.pas"));
                ProgramEntry.ExeProgram(System.IO.Path.Combine("..", "..", "..", "TestData", "Example2.pas"));
                ProgramEntry.ExeProgram(System.IO.Path.Combine("..", "..", "..", "TestData", "Example3.pas"));
            }
        }


    }
}
