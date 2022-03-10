using System;
using System.IO;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            // scanner
            Console.WriteLine("Scanner working...");
            Scanner scanner = new Scanner(Path.Join("..", "..", "..", "TestData", "Example1.pas"));
            // parser
            Console.WriteLine("Parser working...");
            Parser parser = new Parser(scanner);
            Statements stats = parser.BuildAST();
            ErrorHandler.PrintErrors();

            // scanner
            Console.WriteLine("Scanner working...");
            Scanner scanner2 = new Scanner(Path.Join("..", "..", "..", "TestData", "Example2.pas"));
            // parser
            Console.WriteLine("Parser working...");
            Parser parser2 = new Parser(scanner2);
            Statements stats2 = parser2.BuildAST();
            ErrorHandler.PrintErrors();


            // scanner
            Console.WriteLine("Scanner working...");
            Scanner scanner3 = new Scanner(Path.Join("..", "..", "..", "TestData", "Example3.pas"));
            // parser
            Console.WriteLine("Parser working...");
            Parser parser3 = new Parser(scanner3);
            Statements stats3 = parser3.BuildAST();
            ErrorHandler.PrintErrors();
            Console.WriteLine("ok");
        }
    }
}
