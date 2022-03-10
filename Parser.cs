using System;
using System.Collections.Generic;
using System.Text;
using MiniPLCompiler.ASTComponents;

namespace MiniPLCompiler
{
    class Parser
    {
        private Scanner scanner;
        private List<BaseNode> stats = new List<BaseNode>();

        private HashSet<TokenType> startSet = new HashSet<TokenType> 
        { 
            TokenType.VAR,
            TokenType.IDENTIFIER,
            TokenType.FOR,
            TokenType.READ,
            TokenType.PRINT,
            TokenType.ASSERT,
        };

        // init
        public Parser(Scanner sc)
        {
            scanner = sc;
        }

        public Statements BuildAST()
        {
            Token token = scanner.PullOneToken();
            while (token.type != TokenType.END_OF_PROGRAM)
            {
                if (startSet.Contains(token.type))
                {
                    scanner.PushOneToken(token);
                    Statements tmps = (Statements)new Statements().TryBuild(ref scanner);
                    stats.AddRange(tmps.statsList);
                }
                else
                {
                    ErrorHandler.PushError(new MyError(token.lexeme, token.lineNum, "Can't recognize this token as the start of the statement."));
                    SkipHelper.SkipToSemi(ref scanner, token);
                }

                token = scanner.PullOneToken();
            }

            Statements fakeStats = new Statements();
            fakeStats.statsList = stats;
            return fakeStats;
        }

    }
}
