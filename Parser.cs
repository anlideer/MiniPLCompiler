using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class Parser
    {
        private Scanner scanner;

        // init
        public Parser(Scanner sc)
        {
            scanner = sc;
        }

        public PLProgram BuildAST()
        {
            return (PLProgram)new PLProgram().TryBuild(ref scanner);
        }

    }
}
