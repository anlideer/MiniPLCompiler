using System;
using System.IO;
using MiniPLCompiler.ASTComponents;

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
                Scanner scanner = new Scanner(Path.Combine("..", "..", "..", "TestData", "Example1.pas"));

                Token t = scanner.PullOneToken();
                while(t.type != TokenType.END_OF_PROGRAM)
                {
                    Console.WriteLine(string.Format("{0}: {1}, {2}", t.type, t.lexeme, t.lineNum));
                    t = scanner.PullOneToken();
                }

                scanner = new Scanner(Path.Combine("..", "..", "..", "TestData", "Example2.pas"));
                Parser parser = new Parser(scanner);
                PLProgram pro = parser.BuildAST();
                Visitor visitor = new Visitor();
                pro.Accept(visitor);
                ErrorHandler.PrintErrors();

                // continue with the backend
                if (ErrorHandler.IsEmpty())
                {
                    SimpleInterpreter interpreter = new SimpleInterpreter();
                    pro.AcceptExe(interpreter);
                    Console.WriteLine(interpreter.GetOutput());
                }
            }
        }


    }
}
