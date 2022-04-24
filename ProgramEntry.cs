using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    public class ProgramEntry
    {
        public static void ExeProgram(string fileName)
        {
            // scanner
            Scanner scanner = new Scanner(fileName);
            /*
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
            */
        }

    }
}
