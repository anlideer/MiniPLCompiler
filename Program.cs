using System;
using System.IO;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //ExeProgram("Example1.pas");
            //ExeProgram("Example2.pas");
            ExeProgram("Example3.pas");

            //Console.WriteLine("ok");
        }

        private static void ExeProgram(string fileName)
        {
            // scanner
            Scanner scanner = new Scanner(Path.Combine("..", "..", "..", "TestData", fileName));
            // parser
            Parser parser = new Parser(scanner);
            Statements stats = parser.BuildAST();
            ErrorHandler.PrintErrors();
            // semantic analysis (visitor)
            Visitor visitor = new Visitor();
            stats.Accept(visitor);
            ErrorHandler.PrintErrors();
            // interpreter
            if (ErrorHandler.IsEmpty())
            {
                SimpleInterpreter interpreter = new SimpleInterpreter(visitor);
                stats.AcceptExe(interpreter);
            }

            // clear errors
            ErrorHandler.ClearAll();
        }
    }
}
