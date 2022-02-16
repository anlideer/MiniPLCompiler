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
            Token t = scanner.PullOneToken();
            while (t.type != TokenType.END_OF_PROGRAM)
            {
                Console.WriteLine(String.Format("{0}: {1} in line {2}", t.type, t.lexeme, t.lineNum));
                t = scanner.PullOneToken();
            }
        }
    }
}
