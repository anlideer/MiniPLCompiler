using System;
using System.IO;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CharacterHandler cStream = new CharacterHandler(Path.Join("..", "..", "..", "TestData", "Example1.pas"));

        }
    }
}
