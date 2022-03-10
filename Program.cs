using System;
using System.IO;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            ExeProgram("Example1.pas");
            ExeProgram("Example2.pas");
            ExeProgram("Example3.pas");

            Console.WriteLine("ok");
        }

        private static void ExeProgram(string fileName)
        {
            // scanner
            Scanner scanner = new Scanner(Path.Join("..", "..", "..", "TestData", fileName));
            // parser
            Parser parser = new Parser(scanner);
            Statements stats = parser.BuildAST();
            ErrorHandler.PrintErrors();
            // semantic analysis

        }
    }
}
