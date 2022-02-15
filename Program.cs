using System;
using System.IO;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Scanner scanner = new Scanner(Path.Join("..", "..", "..", "TestData", "Example1.pas"));
        }
    }
}
